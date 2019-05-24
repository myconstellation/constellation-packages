/*
 *	 Huawei Mobile Router Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace HuaweiMobileRouter
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Macs;
    using Org.BouncyCastle.Crypto.Parameters;
    using System;
    using System.Linq;
    using System.Text;

    internal static class CryptoUtils
    {
        internal static string ComputeClientProof(string clientnonce, string servernonce, string password, string salt, int iterations)
        {
            byte[] saltedPass = ComputePbkdf2(password, StringToByteArray(salt), iterations, 32);
            byte[] clientKey = ComputeSHA256HMac(saltedPass, Encoding.UTF8.GetBytes("Client Key"));
            byte[] storedKey = ComputeSHA256Hash(clientKey);
            byte[] signature = ComputeSHA256HMac(storedKey, Encoding.UTF8.GetBytes($"{clientnonce},{servernonce},{servernonce}"));

            var clientProof = new byte[clientKey.Length];
            for (int i = 0; i < clientProof.Length; i++)
            {
                clientProof[i] = (byte)(clientKey[i] ^ signature[i]);
            }
            return BitConverter.ToString(clientProof).Replace("-", string.Empty).ToLower();
        }

        internal static byte[] ComputePbkdf2(string password, byte[] salt, int iterations, int hashByteSize)
        {
            var pdb = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            pdb.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, iterations);
            var key = (KeyParameter)pdb.GenerateDerivedMacParameters(hashByteSize * 8);
            return key.GetKey();
        }

        internal static string ComputeSHA256Hash(string msg)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            byte[] hash = ComputeSHA256Hash(msgBytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        internal static byte[] ComputeSHA256HMac(byte[] msg, byte[] password)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(password));
            byte[] result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(msg, 0, msg.Length);
            hmac.DoFinal(result, 0);
            return result;
        }

        internal static byte[] ComputeSHA256Hash(byte[] msg)
        {
            var hash = new Sha256Digest();
            hash.BlockUpdate(msg, 0, msg.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            return result;
        }

        internal static string StringToBase64String(string msg)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
        }

        internal static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}

/*
 *	 OrangeTV Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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
 
 namespace OrangeTV
{
    /// <summary>
    /// Orange TV Channels list
    /// </summary>
    public enum Channel
    {
        [OrangeServiceId(0)]
        Unknown = 0,
        [OrangeServiceId(192)]
        TF1,
        [OrangeServiceId(4)]
        France2,
        [OrangeServiceId(80)]
        France3,
        [OrangeServiceId(34)]
        CanalPlus,
        [OrangeServiceId(47)]
        France5,
        [OrangeServiceId(118)]
        M6,
        [OrangeServiceId(111)]
        Arte,
        [OrangeServiceId(445)]
        C8,
        [OrangeServiceId(119)]
        W9,
        [OrangeServiceId(195)]
        TMC,
        [OrangeServiceId(446)]
        NT1,
        [OrangeServiceId(444)]
        NRJ12,
        [OrangeServiceId(234)]
        LCP,
        [OrangeServiceId(78)]
        France4,
        [OrangeServiceId(481)]
        BFMTV,
        [OrangeServiceId(226)]
        iTele,
        [OrangeServiceId(458)]
        C17,
        [OrangeServiceId(482)]
        Gulli,
        [OrangeServiceId(160)]
        FranceO,
        [OrangeServiceId(1404)]
        HD1,
        [OrangeServiceId(1401)]
        LEquipe21,
        [OrangeServiceId(1403)]
        Sister6ter,
        [OrangeServiceId(1402)]
        Numero23,
        [OrangeServiceId(1400)]
        RMCDecouverte,
        [OrangeServiceId(1399)]
        Cherie25,
        [OrangeServiceId(191)]
        Teva,
        [OrangeServiceId(205)]
        TV5Monde,
        [OrangeServiceId(145)]
        ParisPremiere,
        [OrangeServiceId(115)]
        RTL9,
        [OrangeServiceId(5)]
        AB1,
        [OrangeServiceId(225)]
        TvBreizh,
        [OrangeServiceId(33)]
        CanalPlus_Cinéma,
        [OrangeServiceId(35)]
        CanalPlus_Sport,
        [OrangeServiceId(657)]
        CanalPlus_Family,
        [OrangeServiceId(30)]
        CanalPlus_Decale,
        [OrangeServiceId(730)]
        OCS_max,
        [OrangeServiceId(128)]
        OCS_City,
        [OrangeServiceId(732)]
        OCS_choc,
        [OrangeServiceId(734)]
        OCS_geants,
        [OrangeServiceId(185)]
        TCM,
        [OrangeServiceId(58)]
        DisneyChannel,
        [OrangeServiceId(321)]
        Boomerang,
        [OrangeServiceId(924)]
        Boing,
        [OrangeServiceId(64)]
        EquidiaLive,
        [OrangeServiceId(1146)]
        EquidiaLife,
        [OrangeServiceId(15)]
        ABMoteurs,
        [OrangeServiceId(1290)]
        beIN_SPORT1,
        [OrangeServiceId(1304)]
        beIN_SPORT2,
        [OrangeServiceId(76)]
        Eurosport,
        [OrangeServiceId(439)]
        Eurosport2,
        [OrangeServiceId(1355)]
        sport365,
        [OrangeServiceId(415)]
        NauticalChannel,
        [OrangeServiceId(112)]
        LCI,
        [OrangeServiceId(529)]
        France24,
        [OrangeServiceId(451)]
        UshuaiaTV,
        [OrangeServiceId(88)]
        Histoire,
        [OrangeServiceId(12)]
        Animaux,
        [OrangeServiceId(67)]
        Escales,
        [OrangeServiceId(38)]
        ChasseEtPeche,
        [OrangeServiceId(7)]
        TouteLHistoire,
        [OrangeServiceId(63)]
        Encyclo,
        [OrangeServiceId(1000000)]
        NoLife,
        [OrangeServiceId(87)]
        GameOne,
        [OrangeServiceId(6)]
        Mangas,
        [OrangeServiceId(929)]
        KZTV,
        [OrangeServiceId(325)]
        TRACEUrban,
        [OrangeServiceId(605)]
        NRJHits,
        [OrangeServiceId(453)]
        M6MusicHits,
        [OrangeServiceId(343)]
        MCMTop,
        [OrangeServiceId(241)]
        MCMPop,
        [OrangeServiceId(753)]
        TraceTropical,
        [OrangeServiceId(53)]
        CNN,
        [OrangeServiceId(19)]
        BBCWorld,
        [OrangeServiceId(180)]
        AlJazeeraEnglish,
    }
}
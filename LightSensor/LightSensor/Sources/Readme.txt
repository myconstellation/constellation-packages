Source : http://dino.ciuffetti.info/2014/03/tsl2561-light-sensor-on-raspberry-pi-in-c/

gcc -Wall -O2 -o TSL2561.o -c TSL2561.c
gcc -Wall -O2 -o GetTSL2561.o -c GetTSL2561.c
gcc -Wall -O2 -o GetTSL2561 TSL2561.o GetTSL2561.o
/*
 Source : https://github.com/sebastienwarin/libmodbus
 gcc modbus.c -I '/usr/local/include/modbus' -o modbus -lmodbus
*/

#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <modbus.h>

int main(int argc, char *argv[]) {
    /* Check arguments */
    if (argc < 8) {
        fprintf(stderr, "Invalid arguments\nUsage: %s <server_id> <uart_port> <baud_rate> <bcm_pin_re> <bcm_pin_de> <register_address> <no_of_registers> [debug]", argv[0]);
        return -1;
    }

    /* Parse arguments */
    int server_id = atoi(argv[1]);
    char* uart_port = argv[2];
    int baud_rate = atoi(argv[3]);
    int bcm_pin_re = atoi(argv[4]);
    int bcm_pin_de = atoi(argv[5]);
    int register_address = atoi(argv[6]);
    int no_of_registers = atoi(argv[7]);
    int debug = argc > 8 && strcmp(argv[8], "debug") == 0;
    if (debug) {
        printf("Paramaters:\nServerID: %d\nUART port: %s\nBaud rate: %d\nBCM pin RE: %d\nBCM pin DE: %d\nRegister address: %d\nNumber of registers: %d\n",
            server_id,
            uart_port,
            baud_rate,
            bcm_pin_re,
            bcm_pin_de,
            register_address,
            no_of_registers);
    }

    /* Init modbus lib */
    modbus_t *ctx = NULL;
    uint32_t sec_to = 1;
    uint32_t usec_to = 0;
    ctx = modbus_new_rtu(uart_port, baud_rate, 'N', 8, 1);
    if (ctx == NULL) {
        fprintf(stderr, "Unable to allocate libmodbus context\n");
        return -1;
    }
    modbus_set_debug(ctx, debug);
    modbus_set_error_recovery(ctx, MODBUS_ERROR_RECOVERY_LINK | MODBUS_ERROR_RECOVERY_PROTOCOL);
    modbus_set_slave(ctx, server_id);
    modbus_get_response_timeout(ctx, &sec_to, &usec_to);
    modbus_enable_rpi(ctx, TRUE);
    modbus_configure_rpi_bcm_pins(ctx, bcm_pin_de, bcm_pin_re);
    modbus_rpi_pin_export_direction(ctx);
    if (modbus_connect(ctx) == -1) {
        fprintf(stderr, "Connection failed: %s\n", modbus_strerror(errno));
        modbus_free(ctx);
        return -1;
    }

    /* Read Modbus registers */
    uint16_t *tab_rp_registers = NULL;
    tab_rp_registers = (uint16_t *)malloc(no_of_registers * sizeof(uint16_t));
    memset(tab_rp_registers, 0, no_of_registers * sizeof(uint16_t));
    int rc = modbus_read_registers(ctx, register_address, no_of_registers, tab_rp_registers);

    /* Print registers */
    if (debug) {
        printf("Result:");
    }
    for (int i = 0; i < no_of_registers; i++) {
        printf("%d;", tab_rp_registers[i]);
    }

    /* Free memory */
    free(tab_rp_registers);

    /* Close the connection */
    modbus_rpi_pin_unexport_direction(ctx);
    modbus_close(ctx);
    modbus_free(ctx);
}

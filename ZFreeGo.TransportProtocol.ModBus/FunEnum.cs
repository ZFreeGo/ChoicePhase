using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.Modbus
{
    public enum FunEnum : byte
    {
        None = 0,
        RESET_MCU = 0x01,
        LED1_TOGLE = 0x11,
        LED2_TOGLE = 0x12,
        LED3_TOGLE = 0x13,
        LED4_TOGLE = 0x14,
        LED1_ON = 0x15,
        LED2_ON = 0x16,
        LED3_ON = 0x17,
        LED4_ON = 0x18,
        LED1_OFF = 0x19,
        LED2_OFF = 0x1A,
        LED3_OFF = 0x1B,
        LED4_OFF = 0x1C,
        LED_ALL_ON = 0x1D,
        LED_ALL_OFF = 0x1F,
        
        START_SAMPLE =  0x20,
        STOP_SAMPLE = 0x21,

        GET_SAMPLE_DATA = 0x30,
        GET_REAL_QUENCY = 0x31,
        GET_OVD = 0x32,

        GET_DATA_TP = 0x33,   //
        GET_DATA_PHASE   = 0x34,   //
        GET_DATA_TIMEDIFF = 0x35,   //
        GET_DATA_T0  = 0x36,
        Error = 0xAA,

       

        SET_HEZHA_PHASE = 0x40,// 设置合闸相角 设置项以 0x40 开始。  弧度6.28926
        SET_HEZHA_TIME = 0x41,

        SAMPLE_DATA = 0x51,

        YONGCI_TONGBU = 0x80,
        YONGCI_TONGBU_RESET = 0x81,
        TEST_YONGCI_WAIT = 0x82,
        ACK  = 0xFA,
        //
        SIMPLE_HEZHA = 0x60,
        SIMPLE_FENZHA = 0x61,
    }
}

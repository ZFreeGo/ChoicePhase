using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFreeGo.ChoicePhase.ControlCenter
{
    public enum YongciFunEnum : byte
    {
        None = 0,
        RESET_MCU = 0x01,

        LED1_TOGLE = 0x11,
        LED2_TOGLE = 0x12,
        LED3_TOGLE = 0x13,
        LED4_TOGLE = 0x14,
        LED5_TOGLE = 0x15,
        LED6_TOGLE = 0x16,
        LED7_TOGLE = 0x17,
        LED8_TOGLE = 0x18,

        LED1_ON = 0x19,
        LED2_ON = 0x1A,
        LED3_ON = 0x1B,
        LED4_ON = 0x1C,
        LED5_ON = 0x1D,
        LED6_ON = 0x1E,
        LED7_ON = 0x1F,
        LED8_ON = 0x20,


        LED1_OFF = 0x21,
        LED2_OFF = 0x22,
        LED3_OFF = 0x23,
        LED4_OFF = 0x24,
        LED5_OFF = 0x25,
        LED6_OFF = 0x26,
        LED7_OFF = 0x27,
        LED8_OFF = 0x28,

        HEZHA = 0x30,
        FENZHA = 0x31,
        HEZHA_TIME = 0x32, //合闸时间
        FENZHA_TIME = 0x33, //分闸时间
        TURN_ON_INT0 = 0x34,
        TURN_OFF_INT0 = 0x35,

        CO_ACTION = 0x38,
        O_CO_ACTION = 0x39,

        YONGCI_TONGBU = 0x80,
        YONGCI_TONGBU_RESET = 0x81,
        YONGCI_WAIT_HE_ACTION = 0x85,
        ACK = 0xFA
    }
}

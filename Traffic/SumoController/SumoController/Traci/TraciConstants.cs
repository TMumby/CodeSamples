

namespace SumoController
{
    /// <summary>
    /// Holds all constants/data types for Traci. Values taken from the Traci Python interface, constants.py 
    /// </summary>
    public static class TraciConstants
    {
        public enum Command: byte
        {
            CMD_GETVERSION = 0x00,            
            CMD_SIMSTEP = 0x02,
            CMD_GET_INDUCTIONLOOP_VARIABLE = 0xa0,
            CMD_GET_TL_VARIABLE = 0xa2,
            CMD_SET_TL_VARIABLE = 0xc2
        }

        public enum Variable: byte
        {
            ID_LIST = 0x00,
            LAST_STEP_VEHICLE_ID_LIST = 0x12,
            TL_RED_YELLOW_GREEN_STATE = 0x20,
            TL_CONTROLLED_LANES = 0x26,
            VAR_POSITION = 0x42,
            VAR_LANE_ID = 0x51
        }

        public enum DataType: byte
        {
            TYPE_DOUBLE = 0x0B,
            TYPE_STRINGLIST = 0x0E,
            TYPE_STRING = 0x0C
        }
    }
}

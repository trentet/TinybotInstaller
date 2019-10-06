//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace TinybotInstaller.PowerManagement
//{
//    class Read
//    {
//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadACDefaultIndex(
//            IntPtr RootPowerKey,
//            ref Guid SchemePersonalityGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint AcDefaultIndex);

//        public uint ACDefaultIndex(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadACDefaultIndex(
//                IntPtr.Zero,
//                ref powerSchemeGuid,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadACValueIndex(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint AcValueIndex);

//        public uint ACValueIndex(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadACValueIndex(
//                IntPtr.Zero,
//                ref powerSchemeGuid,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadDCDefaultIndex(
//            IntPtr RootPowerKey,
//            ref Guid SchemePersonalityGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint DcDefaultIndex);

//        public uint DCDefaultIndex(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadDCDefaultIndex(
//                IntPtr.Zero,
//                ref powerSchemeGuid,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadDCValueIndex(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint DcValueIndex);

//        public uint DCValueIndex(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadDCValueIndex(
//                IntPtr.Zero,
//                ref powerSchemeGuid,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadDescription(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref int Buffer,
//            ref uint BufferSize);

//        public uint Description(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            var buffer = 0;
//            var bufferSize = 4u;

//            PowerReadDescription(
//                IntPtr.Zero,
//                ref powerSchemeGuid,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref buffer,
//                ref bufferSize);
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadFriendlyName(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref int Buffer,
//            ref uint BufferSize);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadIconResourceSpecifier(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref int Buffer,
//            ref uint BufferSize);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadPossibleDescription(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint PossibleSettingIndex,
//            ref int Buffer,
//            ref uint BufferSize);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadPossibleFriendlyName(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint PossibleSettingIndex,
//            ref int Buffer,
//            ref uint BufferSize);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadPossibleValue(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint Type,
//            ref uint PossibleSettingIndex,
//            ref int Buffer,
//            ref uint BufferSize);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadSettingAttributes(
//            ref Guid SubGroupGuid,
//            ref Guid PowerSettingGuid);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadValueIncrement(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint ValueIncrement);



//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadValueMax(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint ValueMaximum);

//        public uint ValueMax(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadValueMax(
//                IntPtr.Zero,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadValueMin(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref uint ValueMinimum);

//        public uint ValueMin(Guid powerSchemeGuid, Guid subGroupGuid, Guid powerSettingGuid)
//        {
//            uint value = 0;
//            PowerReadValueMin(
//                IntPtr.Zero,
//                ref subGroupGuid,
//                ref powerSettingGuid,
//                ref value);

//            return value;
//        }

//        [DllImport("powrprof.dll")]
//        private static extern uint PowerReadValueUnitsSpecifier(
//            IntPtr RootPowerKey,
//            ref Guid SubGroupOfPowerSettingsGuid,
//            ref Guid PowerSettingGuid,
//            ref int Buffer,
//            ref uint BufferSize);


//    }
//}

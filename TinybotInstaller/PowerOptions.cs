//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace TinybotInstaller
//{
//    class PowerOptions
//    {
//        //SUB_DISK
//        private static Guid GUID_DISK_SUBGROUP =
//            new Guid("0012ee47-9041-4b5d-9b77-535fba8b1442");

//        //SUB_DISK - DISKIDLE
//        private static Guid GUID_DISK_DISKIDLE =
//            new Guid("6738e2c4-e8a5-4a42-b16a-e040e769756e");

//        //SUB_VIDEO
//        private static Guid GUID_VIDEO_SUBGROUP =
//            new Guid("7516b95f-f776-4464-8c53-06167f40cc99");

//        //SUB_VIDEO - VIDEOIDLE
//        private static Guid GUID_VIDEO_VIDEOIDLE =
//            new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");

//        //SUB_SLEEP
//        private static Guid GUID_SLEEP_SUBGROUP =
//            new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");

//        //SUB_SLEEP - STANDBYIDLE
//        private static Guid GUID_SLEEP_STANDBYIDLE =
//            new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

//        //SUB_SLEEP - HYBRIDSLEEP
//        private static Guid GUID_SLEEP_HYBRIDSLEEP =
//            new Guid("94ac6d29-73ce-41a6-809f-6363ba21b47e");

//        //SUB_SLEEP - HIBERNATEIDLE
//        private static Guid GUID_SLEEP_HIBERNATEIDLE =
//            new Guid("9d7815a6-7ee4-497e-8888-515a05f02364");

//        [DllImport("powrprof.dll")]
//        static extern uint PowerGetActiveScheme(
//            IntPtr UserRootPowerKey,
//            ref IntPtr ActivePolicyGuid);

//        [DllImport("powrprof.dll")]
//        static extern uint PowerReadACValue(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingGuid,
//            ref Guid PowerSettingGuid,
//            ref int Type,
//            ref int Buffer,
//            ref uint BufferSize);

//        [DllImport("powrprof.dll")]
//        static extern void PowerWriteACDefaultIndex(
//            IntPtr RootPowerKey,
//            ref Guid SchemeGuid,
//            ref Guid SubGroupOfPowerSettingGuid,
//            ref Guid PowerSettingGuid,
//            ref int AcValueIndex
//        );

//        public int GetCurrentHibernateIdleValue()
//        {
//            IntPtr activePolicyGuidPTR = IntPtr.Zero;
//            PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

//            var activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
//            var type = 0;
//            var value = 0;
//            var valueSize = 4u;

//            PowerReadACValue(
//                IntPtr.Zero, 
//                ref activePolicyGuid,
//                ref GUID_SLEEP_SUBGROUP, 
//                ref GUID_SLEEP_HIBERNATEIDLE,
//                ref type, 
//                ref value, 
//                ref valueSize);

//            return value;
//        }

//        public void SetCurrentHibernateIdleValue(int minutes)
//        {
//            IntPtr activePolicyGuidPTR = IntPtr.Zero;
//            PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

//            var activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
//            var type = 0;
//            var value = 0;
//            var valueSize = 4u;

//            PowerReadACValue(
//                IntPtr.Zero,
//                ref activePolicyGuid,
//                ref GUID_SLEEP_SUBGROUP,
//                ref GUID_SLEEP_HIBERNATEIDLE,
//                ref type,
//                ref value,
//                ref valueSize);

//            return value;
//        }
//    }
//}

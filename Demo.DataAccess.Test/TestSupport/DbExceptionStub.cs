using System;

namespace Demo.DataAccess.Test.TestSupport
{
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DbExceptionStub : System.Data.Common.DbException
    {
        public DbExceptionStub()
            : base()
        {

        }
    }
}

using System.IO;
using System.Web;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class HttpPostedFileMock : HttpPostedFileBase
    {
        private readonly string _filename;
        private readonly Stream _inputStream;

        public override string FileName
        {
            get { return _filename; }
        }

        public override Stream InputStream
        {
            get { return _inputStream; }
        }

        public HttpPostedFileMock(string filename,Stream inputStream)
        {
            _filename = filename;
            _inputStream = inputStream;
        }
    }
}
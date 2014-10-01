using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionTesting
{
    public class TestableClass
    {
        public bool MySimpleMethod(List<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            return true;
        }

        public async Task<bool> MySimpleMethodAsync(List<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            await Task.Delay(100);

            return true;
        }
    }
}

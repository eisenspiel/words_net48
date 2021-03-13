using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WordsCountingLib;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public Dictionary<string, int> GetData(string text)
        {
            var WordsCountIstance = new WordsCountingClass();
            Dictionary<string, int> wordCount = WordsCountIstance.CountWordsMultithread(text);

            return wordCount;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                var WordsCountIstance = new WordsCountingClass();
                composite.Dict = WordsCountIstance.CountWordsMultithread(composite.StringValue);

                //composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}

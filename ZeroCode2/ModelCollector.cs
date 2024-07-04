using System.Collections.Generic;

namespace ZeroCode2
{
    public class ModelCollector
    {
        public List<Models.ParameterModel> ParameterModels { get; set; }
        public List<Models.SingleModel> SingleModels { get; set; }

        public SortedDictionary<string, string> Variables { get; set; }

        public ModelCollector()
        {
            ParameterModels = new List<Models.ParameterModel>();
            SingleModels = new List<Models.SingleModel>();
            Variables = new SortedDictionary<string, string>();
        }
    }


}

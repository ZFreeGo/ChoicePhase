using System;
using ZFreeGo.ChoicePhase.ControlPlatform.Model;

namespace ZFreeGo.ChoicePhase.ControlPlatform.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}
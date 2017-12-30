using System;
using System.Linq;
using FlowBaseAPI.Models;
using FlowBaseAPI.Globals;

namespace FlowBaseAPI.DataLayer {

    public class FlowbaseDbInitializer
    {
        public static void Seed(FlowbaseContext context) {
            context.Database.EnsureCreated();
            if (context.MetaData.Any()) {
                return;
            }

            var MaxBarcode = Numbers.FirstBarcode;
            var MetaData = new MetaData();
            MetaData.MaxBarcode = MaxBarcode;
            context.MetaData.Add(MetaData);
            context.SaveChanges();
        }
    }

}
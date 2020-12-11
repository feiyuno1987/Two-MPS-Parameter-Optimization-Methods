using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearning_Method
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }
}

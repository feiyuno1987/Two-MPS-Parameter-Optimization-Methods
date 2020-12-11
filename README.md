Paper name is "Two parameter optimization methods of multi-point geostatistics"

The program codes of the two MPS parameter optimization methods in this paper are described as follows. 

The source code of GLCM-based method is written in MATLAB, and Deep Learning program code is written in c# .Net.

1.GLCM-based method
   The main program is "GLCM_Method.m",which depends on "GrayCoMatrix.m" and "HsimSimilarity.m".
   The third-party codes used in GLCM-based method contain "sort_nat.m","rotateticklabel.m".

2.Deep Learning-based method
   The main program is "Program.cs",which is based on ML.Net.This program contains code files ,such as "Pretreatment_ImageFolder" , "ImageNetData.cs" , "MyDataTable.cs" ,etc.

Before using these programs, decompress the two compressed files "demo data.rar" and "ML_Assets.rar". File "demo data.rar" contains the data used in the thesis, including realizations of training image channel and the categorical obtained based on Snesim and simpat.


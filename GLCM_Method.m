%%  Parameter Optimize Method based on GLCM Texture Feature for MPS
%
% The GLCM-based method uses the texture feature statistics to compare the
% difference between MPS realizations and training image, and use Hsim
% similarity function to optimize modeling parameters.
%
% usage:
%   Preparation:
%   1.use training image TI and parameters(radius of tempate for example)
%     to generate a group of realizations,prefix the parameter value before
%     the realization's name,such as "[radius1]_largetrain_101_1.out",where
%     "[radius1]" is the parameter value,"largetrain_101"is the TI name,and
%     the last number "1" is the index of realization.The three parts are
%     divided by "_".
%   2.Convert the model into a digital image in jpg format,and the name
%     remains the same.
%
%   Set code configuration and Run:
%   1.After preparation,manage the files of realizations and training image
%     according to the folder "demo data".The realizations and training image
%     are placed in separate folders.
%     Please unzip the demo data zip file to the directory where the program
%     file is located
%   2.Set the variables Realizations_Dir and TI_Path to ensure that the file
%     path is correct.
%   3.Run the code, and all the work will run automatically.
%   4.After the process is finished, a result chart is automatically created.
%     According to the curve of the graph,select the modeling parameter value.
%
%   Version: 1.0
%   Author:  Siyu Yu
%   Email:   siyuyu@yangtzeu.edu.cn (or 573315294@qq.com)
%   Date:    9 Dec 2020
%%

clc;
clear;
close all;

% test of template radius with simpat,TI is largetrain_101(channel),and MultiGrid is 3
Realizations_Dir='.\demo data\largetrain_101 [template radius MG3]\realizations';
TI_Path='.\demo data\largetrain_101 [template radius MG3]\training image\largetrain_101.jpg';

% test of template radius with simpat,TI is categorical_sgems(categorical),and MultiGrid is 3
% Realizations_Dir='.\demo data\categorical_sgems [template radius MG3]\realizations';
% TI_Path='.\demo data\categorical_sgems [template radius MG3]\training image\categorical_sgems.jpg';

% test of MultiGrid with simpat,TI is largetrain_101(channel),and template radius is 4
% Realizations_Dir='.\demo data\largetrain_101 [multigrid]\realizations';
% TI_Path='.\demo data\largetrain_101 [multigrid]\training image\largetrain_101.jpg';

% test of MultiGrid with simpat,TI is categorical_sgems(categorical),and template radius is 4
% Realizations_Dir='.\demo data\categorical_sgems [multigrid]\realizations';
% TI_Path='.\demo data\categorical_sgems [multigrid]\training image\categorical_sgems.jpg';

% test of node number with snesim,TI is largetrain_101(channel),and MultiGrid is 3
% Realizations_Dir='.\demo data\largetrain_101 [nodes count MG3]\realizations';
% TI_Path='.\demo data\largetrain_101 [nodes count MG3]\training image\largetrain_101.jpg';

% test of node number with snesim,TI is categorical_sgems(categorical),and MultiGrid is 3
% Realizations_Dir='.\demo data\categorical_sgems [node count MG3]\realizations';
% TI_Path='.\demo data\categorical_sgems [node count MG3]\training image\categorical_sgems.jpg';

%compute texture features of the training image based on GLCM
[Contrast,Homogeneity,Correlation,Energy,Entropy]=GrayCoMatrix(TI_Path);
% TI_TextureFeatures=[Contrast,Homogeneity,Correlation,Energy,Entropy];
TI_TextureFeatures=[Contrast,Correlation,Energy,Entropy];
%get file list of all realizations
fileList=dir(fullfile(Realizations_Dir,'*.jpg'));
%natural order sort file names as parameter ascend order
fileNames=sort_nat({fileList.name});
N=length(fileNames);
hsims = struct();
numbers=struct();
%init struct for parameters
for i=1:N
    realizaton_Name=char(fileNames(i));
    strs= strsplit(realizaton_Name,'_');
    prefix=char(strs(1));%get label of parameter value
    prefix = strrep(prefix,'[','');
    prefix = strrep(prefix,']','');
    hsims.(prefix)=-1;
    numbers.(prefix)=0;
end
h=waitbar(0,'GLCM Method Running...');
for i=1:N
    realizaton_Name=fileList(i).name;
    folder=fileList(i).folder;
    strs= strsplit(realizaton_Name,'_');
    prefix=char(strs(1));%label of paramter value
    prefix = strrep(prefix,'[','');
    prefix = strrep(prefix,']','');
    realization_path=[folder ,'\', realizaton_Name];
    [Contrast,Homogeneity,Correlation,Energy,Entropy]=GrayCoMatrix(realization_path);
    %Realization_TextureFeatures=[Contrast,Homogeneity,Correlation,Energy,Entropy];
    Realization_TextureFeatures=[Contrast,Correlation,Energy,Entropy];
    %hsim similarity of texturefeatures between TI and realizations
    hsim=HsimSimilarity(TI_TextureFeatures,Realization_TextureFeatures);
    hsim_old=hsims.(prefix);
    if hsims.(prefix)==-1
        hsims.(prefix)=hsim;
        numbers.(prefix)=1;
    else
        hsims.(prefix)=hsims.(prefix)+hsim;
        numbers.(prefix)=numbers.(prefix)+1;
    end
    str=['GLCM Method Running...',num2str(i/N*100,'%4.2f'),'%'];
    waitbar(i/N,h,str);
end
close(h);

fields=fieldnames(hsims);
%get mean of hsim
for i=1:length(fields)
    field_str=char(fields(i));
    hsims.(field_str)=hsims.(field_str)/numbers.(field_str);
end

%draw plot
x=1:length(fields);
y=cell2mat(struct2cell(hsims));
plot(x,y,'-*');
grid on;
set(gca,'xtick',x);
set(gca,'xticklabel',fields);
if(length(fields)>10)
    TH = rotateticklabel(gca,30); % 30 stands for rotating 30 degrees
else
    xlabel('parameter value');
end
ylabel('hsim between TI and realizations');
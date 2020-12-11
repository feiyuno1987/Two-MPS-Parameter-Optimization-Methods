%  get the mean of GLCM texture features with graycoprops in four directions 
% 
% usage:  [Contrast,Homogeneity,Correlation,Energy,Entropy] = GrayCoMatrix(ImagePath)
% 
% where,
%    input
%       ImagePath is the file path of gray image
%    output
%       Contrast:the intensity contrast between a pixel and its neighbor over 
%                the whole image. Range = [0 (size(GLCM,1)-1)^2].Contrast is 0 
%                for a constant image.
%       Homogeneity:closeness of the distribution of elements in the GLCM to
%                the GLCM diagonal.Range = [0 1]. Homogeneity is 1 for a 
%                diagonal GLCM.
%       Correlation:statistical measure of how correlated a pixel is to its 
%                neighbor over the whole image. Range = [-1 1].Correlation 
%                is 1 or -1 for a perfectly positively or negatively correlated
%                image. Correlation is NaN for a constant image.
%       Energy:summation of squared elements in the GLCM. Range = [0 1].Energy 
%                is 1 for a constant image.
%       Entropy:used to measure the average information content of gray image 
%                and reflect the randomness of image texture.
%
%   Version: 1.0
%   Author:  Siyu Yu
%   Email:   siyuyu@yangtzeu.edu.cn (or 573315294@qq.com)
%   Date:    9 Dec 2020

function [Contrast,Homogeneity,Correlation,Energy,Entropy]=GrayCoMatrix(ImagePath)
I=imread(ImagePath);
G=rgb2gray(I);
imshow(G);
GLCM=graycomatrix(G, 'GrayLimits' ,[],'offset',[0 1; -1 1; -1 0; -1 -1]);
result=graycoprops(GLCM,{'Contrast','Homogeneity','Correlation','Energy'});

Contrast=mean(result.Contrast);
Homogeneity=mean(result.Homogeneity);
Correlation=mean(result.Correlation);
Energy=mean(result.Energy);

%Normalize the co-occurrence matrix
for n = 1:4
    GLCM(:,:,n) = GLCM(:,:,n)/sum(sum(GLCM(:,:,n)));
end

ent1=0;
ent2=0;
ent3=0;
ent4=0;
for i = 1:8
    for j = 1:8
        if GLCM(i,j,1)~=0
            ent1 = -GLCM(i,j,1)*log(GLCM(i,j,1))+ent1;
            ent2 = -GLCM(i,j,2)*log(GLCM(i,j,2))+ent2;
            ent3 = -GLCM(i,j,3)*log(GLCM(i,j,3))+ent3;
            ent4 = -GLCM(i,j,4)*log(GLCM(i,j,4))+ent4;
        end
    end
end

Entropy=(ent1+ent2+ent3+ent4)/4;

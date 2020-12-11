%    Calculation of Hsim Similarity between Two Vectors
% 
% Hsim similarity function overcomes the problem that the contrast performance
% of general Lp distance function decreases in high-dimensional space, and can
% also avoid the influence of extreme value. The value range of hsim is between
% 0 and 1.
% 
% usage:  [hsim] = HsimSimilarity(vector1,vector2)
% where,
%    vector1 and vector2 are array.
%    hsim is the Hsim similarity between vector1 and vector2.
%
%   Version: 1.0
%   Author:  Siyu Yu
%   Email:   siyuyu@yangtzeu.edu.cn (or 573315294@qq.com)
%   Date:    9 Dec 2020

function [hsim]=HsimSimilarity(vector1,vector2)
if(length(vector1) ~= length(vector2))
    error('the number of elements of Vector 1 and Vector 2 is different');
end
d=0;
N=length(vector1);
for i=1:N
    if(isnan(vector1(i))||isnan(vector2(i)))
        continue;
    end
    d = d + 1.0 / (1.0 + abs(vector1(i) - vector2(i)));
end
hsim=d / N;

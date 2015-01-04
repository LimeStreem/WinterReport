//RWTexture2D<float> Output;
//Texture1D<float> bVec:register(t0);
//Texture2D<float> aMat:register(t1);
//Texture2D<float> xVec:register(t2);
//cbuffer ArgumentConstant:register(b0)
//{
//	uint N;
//	uint A1;
//	uint A2;
//	uint A3;
//}
//float getXi(uint x,uint y)
//{
//	float sum = 0;
//	uint xSc = x + y*N;
//	for (uint i = 0; i < N; i++)
//	{
//		for (uint j= 0; j < N; j++)
//		{
//			if (i == x&&j == y)continue;
//			sum += aMat[uint2(xSc, i*N+j)] * xVec[uint2(j,i)];
//		}
//	}
//	return (bVec[xSc] - sum) / aMat[uint2(xSc, xSc)];
//}
//
//[numthreads(32, 32, 1)]
//void main(uint3 threadID : SV_DispatchThreadID)
//{
//	Output[uint2(threadID.x,threadID.y)] = getXi(threadID.x,threadID.y);
//}

RWTexture1D<float> Output;
Texture1D<float> bVec:register(t0);
Texture2D<float> aMat:register(t1);
Texture1D<float> xVec:register(t2);
cbuffer ArgumentConstant:register(b0)
{
	uint N;
	uint A1;
	uint A2;
	uint A3;
}
float getXi(uint i)
{
	float sum = 0;
	for (uint j = 0; j <N; j++)
	{
		if (i == j)continue;
		sum += aMat[uint2(i, j)] * xVec[j];
	}
	return (bVec[i] - sum) / aMat[uint2(i, i)];
}
[numthreads(1024,1, 1)]
void main(uint3 threadID : SV_DispatchThreadID)
{
	Output[threadID.x] = getXi(threadID.x);
}





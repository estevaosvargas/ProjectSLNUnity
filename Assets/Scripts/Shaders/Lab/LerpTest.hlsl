void LerpAdvanced_float (float4 A, float4 B,float4 C, float1 Input, out float4 Out)
{
   if (Input == float1(0)){
	Out = A;
   }
   else if (Input == float1(1)){
	Out = B;
   }else if (Input == float1(2)){
	Out = C;
   }
}
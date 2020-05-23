void Numsei_float (float4 C1, float4 C2, float4 Input,out float1 Out)
{
   if (C1.x == Input.x && C1.y == Input.y && C1.z == Input.z){
	 Out = float1(0);
   }else if (C2.x == Input.x && C2.y == Input.y && C2.z == Input.z){
	 Out = float1(1);
   }else{
	Out = float1(0);
   }
}
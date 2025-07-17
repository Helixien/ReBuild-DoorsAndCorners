using Verse;

namespace ReBuildDoorsAndCorners
{
    public class WallDiagonalExtension : DefModExtension
    {
        public string texPath_Corner_NW;
        public string texPath_Corner_NE;
        public string texPath_Corner_SW;
        public string texPath_Corner_SE;

        public string texPath_Diagonal_NW;
        public string texPath_Diagonal_NE;
        public string texPath_Diagonal_SW;
        public string texPath_Diagonal_SE;
        public float cornerScale = 2.0f;
        public ShaderTypeDef shaderType;
        public float altitude = -1f;
    }
}

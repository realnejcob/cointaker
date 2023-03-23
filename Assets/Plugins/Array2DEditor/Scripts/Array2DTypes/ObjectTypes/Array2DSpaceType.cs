using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DSpaceType : Array2D<SpaceType>
    {
        [SerializeField]
        CellRowSpaceType[] cells = new CellRowSpaceType[Consts.defaultGridSize];

        protected override CellRow<SpaceType> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
    
    [System.Serializable]
    public class CellRowSpaceType : CellRow<SpaceType> { }
}

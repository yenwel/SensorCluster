namespace SensorCluster.Repository.DAO
{
    [PetaPoco.TableName("aalocguid")]
    [PetaPoco.PrimaryKey("id", AutoIncrement = false)]
    public class aalocguid
    {
        public string id { get; set; }
        public int loclocid { get; set; }
    }
}

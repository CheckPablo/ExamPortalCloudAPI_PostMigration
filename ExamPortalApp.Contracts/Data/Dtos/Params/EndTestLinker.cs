namespace ExamPortalApp.Contracts.Data.Dtos.Params
{
    public class EndTestLinker
    {
        public int TestId { get; set; }
        public int[] StudentIds { get; set; } = new int[0];
    
    }
}
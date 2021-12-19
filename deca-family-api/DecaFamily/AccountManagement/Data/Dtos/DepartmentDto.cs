namespace AccountManagement.Data.Dtos
{
    public class DepartmentDto
    {
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        public CompanyDto Company { get; set; }
    }
}

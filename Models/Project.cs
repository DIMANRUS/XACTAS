namespace Models {
    public class Project {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string VsProjectPath { get; set; }
        public string VsProjectPathSln { get; set; }
    }
}
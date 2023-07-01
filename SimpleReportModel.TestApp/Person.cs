namespace SimpleReportModel.TestApp
{
  public class Person
  {
    public int Id;
    public string Name;
    public string NickName;
  }

  public class Relation
  {
    public Person Parent;
    public Person[] Kids;
  }
}

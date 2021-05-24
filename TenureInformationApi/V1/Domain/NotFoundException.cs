namespace TenureInformationApi.V1.Domain
{
  public class NotFoundException : System.Exception
  {
      public NotFoundException() { }
      public NotFoundException(string message) : base(message) { }
  }
}
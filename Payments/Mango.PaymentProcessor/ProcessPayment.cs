namespace Mango.PaymentProcessor;
public class ProcessPayment : IProcessPayment
{
    public bool PaymentProcessor()
    {
        return true;
    }
}
public interface IProcessPayment
{
    bool PaymentProcessor();
}
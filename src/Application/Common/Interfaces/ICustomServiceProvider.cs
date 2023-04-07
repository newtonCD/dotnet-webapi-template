namespace Application.Common.Interfaces;

public interface ICustomServiceProvider
{
    T GetService<T>();
}
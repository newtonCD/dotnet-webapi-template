using Newtonsoft.Json;

namespace Application.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Extensão genérica que cria uma cópia profunda de um objeto fornecido. Uma cópia profunda significa que,
    /// em vez de apenas copiar as referências para os objetos internos, o método cria uma nova instância do objeto e
    /// todas as suas propriedades e campos. Isso resulta em uma nova estrutura de objeto que é uma réplica exata do
    /// objeto original, mas sem compartilhar referências com ele.
    /// </summary>
    /// <typeparam name="T">O Type.</typeparam>
    /// <param name="source"></param>
    public static T DeepCopy<T>(this T source)
    {
        if (source is null) return default;

        string serializedObject = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<T>(serializedObject);
    }
}
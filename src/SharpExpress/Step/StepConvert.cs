using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;


namespace SharpExpress.Step
{
    public static class StepConvert
    {
        private static Dictionary<Type, Action<object, IReadOnlyList<StepValue>>> mappers_
            = new Dictionary<Type, Action<object, IReadOnlyList<StepValue>>>();
        private static Dictionary<Type, Func<StepValue, object>> converters_
            = new Dictionary<Type, Func<StepValue, object>>();

        public static StepConvertSettings DefaultSettings { get; set; } = new StepConvertSettings();

        public static void PopulateObject<T>(T target, IReadOnlyList<StepValue> values) where T : class
        {
            if (!mappers_.TryGetValue(typeof(T), out var mapper))
            {
                mappers_.Add(typeof(T), mapper = CreateMapper(typeof(T)));
            }

            mapper(target, values);
        }

        private static Action<object, IReadOnlyList<StepValue>> CreateMapper(Type type)
        {
            var objectParameter = Parameter(typeof(object));
            var typedParameter = Convert(objectParameter, type);
            var inputParameter = Parameter(typeof(IReadOnlyList<StepValue>));

            var expressions = new List<System.Linq.Expressions.Expression>();

            var attributes = (from property in type.GetProperties()
                              where Attribute.IsDefined(property, typeof(StepAttributeAttribute))
                              orderby property.GetCustomAttribute<StepAttributeAttribute>().Order
                              select property).ToArray();

            //check length
            var lengthCheck = IfThen(
                NotEqual(
                    Property(inputParameter, typeof(IReadOnlyCollection<StepValue>).GetProperty("Count")),
                    Constant(attributes.Count())),
                Throw(New(typeof(NotImplementedException))));
            
            expressions.Add(lengthCheck);

            for(int i=0; i<attributes.Length; i++)
            {
                var attribute = attributes[i];

                var assignment =
                    Assign(
                        Property(typedParameter, attribute),
                        Convert(Invoke(
                            Constant(GetOrCreateConverter(attribute.PropertyType)),
                            Property(inputParameter, "Item", Constant(i))
                            ), attribute.PropertyType));


                expressions.Add(assignment);
            }

            var body = Block(expressions.ToArray());

            return Lambda<Action<object, IReadOnlyList<StepValue>>>(body, objectParameter, inputParameter).Compile();
        }

        public static T ToObject<T>(StepValue value)
        {
            return (T)GetOrCreateConverter(typeof(T))(value);
        }

        private static Func<StepValue, object> GetOrCreateConverter(Type type)
        {
            if (!converters_.TryGetValue(type, out var result))
                converters_[type] = result = CreateConverter(type);
            return result;
        }

        private static Func<StepValue, object> CreateConverter(Type type)
        {
            if (type == typeof(string))
                return ToString;
            else if (IsListType(type, out var elementType))
            {
                return CreateListConverter(elementType);
            }
            else

                throw new NotImplementedException();
        }

        private static Func<StepValue, object> CreateListConverter(Type elementType)
        {
            var elementConverter = GetOrCreateConverter(elementType);

            var method = typeof(StepConvert).GetMethod(
                nameof(StepConvert.ConvertToList),
                BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(elementType);

            var resultType = typeof(Func<StepValue, Func<StepValue, object>, object>);

            var result = (Func<StepValue, Func<StepValue, object>, object>)
                Delegate.CreateDelegate(resultType, method);

            return value => result(value, elementConverter);
        }

        private static bool IsListType(Type type, out Type elementType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(List<>) ||
                genericTypeDefinition == typeof(IList<>) ||
                genericTypeDefinition == typeof(IReadOnlyList<>))
            {
                elementType = type.GetGenericArguments().Single();
                return true;
            }

            elementType = null;
            return false;
        }

        public static List<T> ToList<T>(StepValue value)
        {
            return ConvertToList<T>(value, GetOrCreateConverter(typeof(T)));
        }

        private static List<T> ConvertToList<T>(StepValue value, Func<StepValue, object> elementMapper)
        {
            if (value.Kind == StepValueKind.List)
            {
                var list = (ListValue)value;
                var result = new List<T>();
                foreach (var item in list.Items)
                    result.Add((T)elementMapper(item));
                return result;
            }
            else if (value.Kind == StepValueKind.Missing)
            {
                return null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static string ToString(StepValue value)
        {
            if (value.Kind == StepValueKind.String)
                return ((StringValue)value).Value;
            else if (value.Kind == StepValueKind.Missing)
                return null;
            else
                //TODO: error
                throw new NotSupportedException();
        }


    }
}

namespace todo.Helpers
{
    public class MapObjects
    {
        public static void Assign<T1, T2>(T1 obj1, T2 obj2)
        {
            var tmp1 = typeof(T1).GetProperties();
            var tmp2 = typeof(T2).GetProperties();
            foreach (var prop2 in tmp2)
            {
                foreach (var prop1 in tmp1)
                {
                    if (prop2.GetValue(obj2) == null)
                    {
                        continue;
                    }
                    if (prop1.Name == prop2.Name)
                    {
                        prop1.SetValue(obj1, prop2.GetValue(obj2));
                        break;
                    }
                }
            }
        }
    }
}

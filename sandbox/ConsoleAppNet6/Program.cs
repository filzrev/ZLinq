
using ZLinq;

[assembly: ZLinq.ZLinqDropInAttribute("", ZLinq.DropInGenerateTypes.Everything, DisableEmitSource = false)]


Enumerable.Range(1, 10).AsValueEnumerable().ToList();
Enumerable.Range(1, 10).AsValueEnumerable().ToList();

//for (int i = 0; i < 100000; i++)
//{
//    if (i % 100 == 0) Console.WriteLine(i);
//    Enumerable.Range(1, i).AsValueEnumerable().ToList();
//    Enumerable.Range(1, i).ToArray().AsValueEnumerable().ToList();
//}

using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;

var obj = JsonNode.Parse(
    """
	{
	"value": 0,
	"inner": [
		{
			"value": 1
		},
		{
			"value": 2,
			"inner": [
				{
					"missing": true
				}
			]
		}
	]
	}
	"""
);

var success = obj!.DescendantsAndSelf(expandArray: true);
//.Select(x => x.Node);
//.SelectMany(o => o.Node?.GetValueKind() == JsonValueKind.Array
//                ? o.Node.AsArray()
//                : Enumerable.Repeat(o.Node, 1)
//)
//.Any(o => o?.GetValueKind() == JsonValueKind.Object && o.AsObject().ContainsKey("missing"));

var huga = success.ToArray();


foreach (var item in huga)
{
    Console.WriteLine(item.Name + ":" + item.Node);
    Console.WriteLine("-----------");
}
// Console.WriteLine($"Success = {success}");

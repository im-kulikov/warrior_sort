using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    [SerializeField] private TextAsset dataPrefab;
    [SerializeField] private GameObject roundPrefab;
    [SerializeField] private GameObject elementPrefab;

    [SerializeField] private Button next;
    [SerializeField] private Button prev;
    [SerializeField] private Button kick;
    [SerializeField] private GameObject view;
    [SerializeField] private Toggle needDebug;

    private int _offset;

    private List<Warrior> _items;
    private Dictionary<int, List<Warrior>> _test;

    private const int Count = 20;

    private int ItemsInRound => _items.Distinct().Count();

    private List<Warrior> Calculate()
    {
        var offset = _offset % _items.Count;
        var round = (_offset / _items.Count) + 1;

        _items.Sort(new WarriorComparer(round));
        var items = _items.GetRange(offset, _items.Count - offset);


        for (var current = round + 1; items.Count < Count; current++)
        {
            _items.Sort(new WarriorComparer(current));

            var count = Count - items.Count;
            if (count > _items.Count)
            {
                count = _items.Count;
            }

            var append = _items.GetRange(0, count);

            for (var j = 0; j < append.Count && items.Count < Count; j++)
            {
                items.Add(append[j % append.Count]);
            }
        }

        return items;
    }

    private void Render()
    {
        foreach (Transform child in view.transform)
        {
            Destroy(child.gameObject);
        }

        var items = Calculate();
        var show = _offset % ItemsInRound < 9;
        for (var i = 0; i < Count; ++i)
        {
            var round = (i + _offset) / ItemsInRound + 1;

            var index = i + _offset;

            if (show || index % _items.Count == 0)
            {
                show = false;

                var obj = Instantiate(roundPrefab, view.transform);
                obj.GetComponentInChildren<Text>().text = $"Раунд {round}";
            }

            var elem = Instantiate(elementPrefab, view.transform);
            var valid = _test[round % 2][index % _items.Count];
            elem.GetComponent<Element>().Propagate(items[i], valid, i + _offset, round, needDebug.isOn);
        }
    }

    private void OnNext()
    {
        _offset++;

        if (_offset > 0)
        {
            prev.enabled = true;
            prev.interactable = true;
        }

        Render();
    }

    private void OnPrev()
    {
        _offset--;

        if (_offset <= 0)
        {
            prev.enabled = false;
            prev.interactable = false;
        }

        Render();
    }

    private void OnKick()
    {
        if (_items.Count <= 1)
        {
            return;
        }

        var items = Calculate();
        var drops = items.ElementAt(1);

        // удаляем из списка
        _items = _items.Where(item => !item.IsSame(drops)).ToList();

        // удаляем из тестовых данных
        _test[0] = _test[0].Where(item => !item.IsSame(drops)).ToList();
        _test[1] = _test[1].Where(item => !item.IsSame(drops)).ToList();

        Render();
    }

    private void OnChangeDebug(bool _)
    {
        Render();
    }

    public void Awake()
    {
        next.onClick.AddListener(OnNext);
        prev.onClick.AddListener(OnPrev);
        kick.onClick.AddListener(OnKick);

        needDebug.onValueChanged.AddListener(OnChangeDebug);

        prev.enabled = false;
        prev.interactable = false;

        {
            // Загружаем данные из JSON
            var data = JsonUtility.FromJson<WarriorData>(dataPrefab.text);

            // воины:
            _items = data.warriors;

            // данные для проверки:
            _test = new Dictionary<int, List<Warrior>>()
            {
                {0, data.second},
                {1, data.first},
            };
        }

        Render();
    }
}
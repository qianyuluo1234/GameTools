using UnityEngine;

namespace GameTools
{
    public interface IPoolBase
    {
        Object Load();

        void Save(Object o);

        void Destroy(Object o);

        void Clear(bool isDestroy);
    }
}
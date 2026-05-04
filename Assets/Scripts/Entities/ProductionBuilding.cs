using UnityEngine;
namespace SlotDefense
{
    public class ProductionBuilding : BuildingController
    {
        private float _timer;

        private void Update()
        {
            if (_data == null || GameManager.Instance == null) return;
            _timer += Time.deltaTime;

            if (_data.buildingType == BuildingType.ProductionEnergy)
            {
                if (_timer >= 1f)
                {
                    _timer -= 1f;
                    int amt = Mathf.RoundToInt(_data.energyPerSecond);
                    GameManager.Instance.ElementalEnergy.AddByType(_data.energyType, amt);
                }
            }
            else if (_data.buildingType == BuildingType.ProductionUnit)
            {
                if (_timer >= _data.spawnInterval && _data.unitToSpawn != null)
                {
                    _timer = 0f;
                    SpawnUnit();
                }
            }
        }

        private void SpawnUnit()
        {
            var arena = FindObjectOfType<ArenaSystem>();
            if (arena == null) return;
            var prefab = _data.unitToSpawn.unitPrefab != null
                ? _data.unitToSpawn.unitPrefab
                : arena.unitPrefab;
            var go = Instantiate(prefab, transform.position + Vector3.left * 0.5f, Quaternion.identity);
            go.GetComponent<UnitController>().Init(_data.unitToSpawn.unitStats, isPlayerUnit: true, portal: arena.portal);
            go.SetActive(true);
        }
    }
}

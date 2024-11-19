using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject towerPrefab; // 설치할 타워 프리팹
    [SerializeField] private LayerMask towerLayer;    // 타워 레이어 설정
    [SerializeField] private LayerMask characterLayer; // 캐릭터 레이어 설정
    [SerializeField] private float maxPlacementDistance = 5f; // 설치 가능한 최대 거리

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            PlaceTower();
        }
    }

    void PlaceTower()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        // 클릭한 셀에 타일이 있는지 확인
        if (tilemap.HasTile(cellPosition))
        {
            // 셀의 중심 월드 좌표 계산
            Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

            Collider2D towerHitCollider = Physics2D.OverlapPoint(cellCenterWorld, towerLayer);
            Collider2D characterHitCollider = Physics2D.OverlapPoint(cellCenterWorld, characterLayer);
            
            if(towerHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 타워가 있어 설치가 불가능합니다.");
                return;
            }
            else if(characterHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 캐릭터가 있어 설치가 불가능합니다.");
                return;
            }

            // 캐릭터와의 거리 계산
            float distance = Vector3.Distance(transform.position, cellCenterWorld);

            if(distance <= maxPlacementDistance)
            {
                // 최대 거리 안이면 타워 생성
                Instantiate(towerPrefab, cellCenterWorld, Quaternion.identity);
                Debug.Log($"타워가 {cellPosition} 위치에 설치되었습니다.");
            }
            else
            {
                Debug.Log("거리가 너무 멉니다.");
            }
        }
    }
}
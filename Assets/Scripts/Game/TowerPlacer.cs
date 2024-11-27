using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacer : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private GameObject towerPrefab; // 설치할 타워 프리팹
    [SerializeField] private float maxPlacementDistance = 5f; // 설치 가능한 최대 거리

    [SerializeField] private GameObject isValidTile; // 설치 가능한 타일 색상
    [SerializeField] private GameObject isUnvalidTile; // 설치 불가능한 타일 색상
    private GameObject currentHighlight;
    private Vector3Int previousCellPosition = Vector3Int.zero;

    void Start()
    {
        tilemap = Utilities.FindAndAssign<Tilemap>("Grid/Tile");
    }

    void Update()
    {
        if (TowerPlacementManager.instance.IsPlacementActive())
        {
            HighlightTile();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭
            {
                PlaceTower();
                Destroy(currentHighlight);
            }
        }

    }

    private void PlaceTower()
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

            Collider2D towerHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Tower"));
            Collider2D characterHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Character"));
            Collider2D enemyHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Enemy"));
            Collider2D obstacleHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Obstacle"));

            if (towerHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 타워가 있어 설치가 불가능합니다.");
                return;
            }
            else if (characterHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 캐릭터가 있어 설치가 불가능합니다.");
                return;
            }
            else if (enemyHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 적이 있어 설치가 불가능합니다.");
                return;
            }
            else if (obstacleHitCollider != null)
            {
                Debug.Log("설치하려는 타일에 장애물이 있어 설치가 불가능합니다.");
                return;
            }

            // 캐릭터와의 거리 계산
            float distance = Vector3.Distance(transform.position, cellCenterWorld);

            if (distance <= maxPlacementDistance)
            {
                // 최대 거리 안이면 타워 생성
                Instantiate(towerPrefab, cellCenterWorld, Quaternion.identity);
                Debug.Log($"타워가 {cellPosition} 위치에 설치되었습니다.");
                TowerPlacementManager.instance.SetPlacementState(false);
            }
            else
            {
                Debug.Log("거리가 너무 멉니다.");
            }
        }
    }

    private void HighlightTile()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        // 마우스가 새로운 타일에 위치했을 때만 처리
        if (cellPosition != previousCellPosition)
        {
            // 기존 하이라이트 제거
            if (currentHighlight != null)
            {
                Destroy(currentHighlight);
            }

            // 현재 타일을 하이라이트
            if (tilemap.HasTile(cellPosition))
            {
                Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

                Collider2D towerHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Tower"));
                Collider2D characterHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Character"));
                Collider2D enemyHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Enemy"));
                Collider2D obstacleHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Obstacle"));

                float distance = Vector3.Distance(transform.position, cellCenterWorld);

                // 설치 가능 여부에 따라 적절한 하이라이트 생성
                if (towerHitCollider == null && characterHitCollider == null && enemyHitCollider == null &&
                    obstacleHitCollider == null && distance <= maxPlacementDistance)
                {
                    currentHighlight = Instantiate(isValidTile, cellCenterWorld, Quaternion.identity);
                }
                else
                {
                    currentHighlight = Instantiate(isUnvalidTile, cellCenterWorld, Quaternion.identity);
                }
            }

            // 새로운 타일 위치 업데이트
            previousCellPosition = cellPosition;
        }
    }
}
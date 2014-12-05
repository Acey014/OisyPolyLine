using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ポリラインを生成し、管理するクラス 
/// </summary>
public class OisyPolyLine : MonoBehaviour {

	/// <summary>
	/// 出力するか
	/// </summary>
	[SerializeField]
	public bool emit = true;

	/// <summary>
	/// 時間経過に応じてポリラインのアルファ値をいじる
	/// </summary>
	[SerializeField]
	bool alphaGraduation = true;

	/// <summary>
	/// 生成されるポリゴンの持続時間 
	/// </summary>
	[SerializeField]
	float lifeTime = 0.5f;

	[SerializeField]
	Transform basePoint;

	[SerializeField]
	Transform tipPoint;

	[SerializeField]
	Color color;

	[SerializeField]
	Material material;

	[System.Serializable]
	public class Line{
		public float timeStamp;
		public float alpha;
		public Vector3 basePosition;
		public Vector3 tipPosition;
	}

	List<Line> lineList;
	GameObject polyLineObject;
	Mesh mesh;

	/// <summary>
	/// UVを適用する最初の座標を決めるためのスイッチ
	/// 毎フレーム切り替わる
	/// </summary>
	bool toggle = false;

	// Use this for initialization
	void Start () {
	
		polyLineObject = new GameObject();
		//マテリアルの指定がない場合はデフォルトのやつを適用する.
		if(material == null)
			material = Resources.GetBuiltinResource<Material>( "Sprites-Default.mat" );

		//polyLineObject.transform.parent = gameObject.transform;
		polyLineObject.transform.parent = null;
		polyLineObject.name = "PolyLine";
		polyLineObject.transform.localPosition = Vector3.zero;
		polyLineObject.transform.localRotation = Quaternion.identity;
		polyLineObject.transform.localScale = Vector3.one;
		polyLineObject.AddComponent(typeof(MeshFilter));
		polyLineObject.AddComponent(typeof(MeshRenderer));
		polyLineObject.renderer.material = material;

		mesh = new Mesh();
		polyLineObject.GetComponent<MeshFilter>().mesh = mesh;

		lineList = new List<Line>();
	}

	void OnDisable()
	{
		Destroy(polyLineObject);
	}
	
	// Update is called once per frame
	void Update () {

		if(lifeTime <= 0)
			alphaGraduation = false;
	
		if(emit)
			UpdateLine();

		if(lineList.Count > 0)
			ApplyMesh();

		//次フレームのUV開始位置を切り替え.
		toggle = !toggle;
	}

	/// <summary>
	/// ポリラインの生成情報を更新する
	/// </summary>
	void UpdateLine(){
		Line newLine = new Line();
		newLine.basePosition = basePoint.position;
		newLine.tipPosition = tipPoint.position;
		newLine.timeStamp = Time.time;
		newLine.alpha = 1;

		//配列の先頭に挿入 
		lineList.Insert(0,newLine);

		//有効期限の切れたポリライン情報を削除.
		for(int i = 0; i < lineList.Count; i++){
			if(Time.time - lineList[i].timeStamp >= lifeTime){
				lineList.RemoveAt(i);
				i--;
			}
			else if(alphaGraduation){
				lineList[i].alpha = 1 - ((Time.time - lineList[i].timeStamp) / lifeTime);
			}

		}
	}

	/// <summary>
	/// 更新したポリライン生成情報をもとにMeshを作り直す
	/// </summary>
	void ApplyMesh(){

		Vector3[] newVertices = new Vector3[lineList.Count * 2];
		Vector2[] newUV = new Vector2[lineList.Count * 2];
		int[] newTriangles = new int[(lineList.Count - 1) * 6];
		Color[] newColors = new Color[lineList.Count * 2];

		for(int i = 0; i < lineList.Count; i++){

			//頂点.
			newVertices[i * 2] = lineList[i].basePosition;
			newVertices[(i * 2) + 1] = lineList[i].tipPosition;

			//頂点カラーはひとまず単色で.
			newColors[i * 2] = color;
			newColors[1 + (i * 2)] = color;

			//アルファを適用 
			newColors[i * 2].a *= lineList[i].alpha;
			newColors[1 + (i * 2)].a *= lineList[i].alpha;

			//UVの設定(ラインごとに反復運動するようなUVを設定する).
			if((!toggle && i%2 == 0) || (toggle && i%2 == 1)){
				newUV[i * 2] = new Vector2(0, 0);
				newUV[1 + (i * 2)] = new Vector2(0, 1);
			}
			else{
				newUV[i * 2] = new Vector2(1, 0);
				newUV[1 + (i * 2)] = new Vector2(1, 1);
			}

			//float uvRatio = (float)i/pointsToUse.Count;
			//newUV[i * 2] = new Vector2(uvRatio, 0);
			//newUV[(i * 2) + 1] = new Vector2(uvRatio, 1);

			//iが1以上(三角ポリが二つ張れる状態)ならTriAnglesを設定.
			if (i > 0)
			{
				newTriangles[(i - 1) * 6] = (i * 2) - 2;
				newTriangles[((i - 1) * 6) + 1] = (i * 2) - 1;
				newTriangles[((i - 1) * 6) + 2] = i * 2;
				
				newTriangles[((i - 1) * 6) + 3] = (i * 2) + 1;
				newTriangles[((i - 1) * 6) + 4] = i * 2;
				newTriangles[((i - 1) * 6) + 5] = (i * 2) - 1;
			}
		}

		//Meshに適用. 
		mesh.Clear();
		mesh.vertices = newVertices;
		mesh.colors = newColors;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;

	}
}

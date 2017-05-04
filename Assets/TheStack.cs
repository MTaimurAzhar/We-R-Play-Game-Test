using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TheStack : MonoBehaviour {
	public Text scoretext, scoreText_1, maxScoreText;
	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	public GameObject missionFailedPanel;
	public GameObject[] theStack;

	private const float BOUNDS_SIZE = 3.5f;
	private const float ERROR_MARGIN = 0.1f;
	public const float STACK_MOVING_SPEED = 5.0f;
	public const float STACK_BOUNDS_GAIN = 0.25f;
	public const int COMBO_START_GAIN = 3;
	public int scoreCount = 0;
	public int maxScore;
	public int stackIndex;
	public int combo = 0;

	public float tileTransition = 0.0f;
	public float tileSpeed = 2.5f;
	public float secondaryPosition ;
	
	public bool isMovingOnX = true;
	public bool gameOver = false;

	public Vector3 desiredPosition;
	public Vector3 lastTilePosition;
	public Vector2 stackBounds = new Vector2(BOUNDS_SIZE , BOUNDS_SIZE);

	// Use this for initialization
	void Start () {
		gameOver = false;
		maxScore = PlayerPrefs.GetInt ("maxScore");
		
		theStack = new GameObject[transform.childCount];
	//	scoreCount = transform.childCount;
		for (int i = 0; i < transform.childCount; i++) {
			theStack [i] = transform.GetChild (i).gameObject;
		//	ColorMesh (theStack [i].GetComponent<MeshFilter> ().mesh);
		//	scoreCount++;
		}
		stackIndex = transform.childCount - 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (PlaceTile ()) 
			{
				SpawnTile ();
				scoreCount ++;
				scoretext.text = scoreCount.ToString ();
			}
			else
			{
				EndGame ();
			}
		
		}

		MoveTile ();
		// move the stack
		transform.position = Vector3.Lerp (transform.position,desiredPosition,STACK_MOVING_SPEED* Time.deltaTime );

	}


	public void SpawnTile()
	{
		lastTilePosition = theStack [stackIndex].transform.localPosition;
		stackIndex--;
		if(stackIndex < 0)
		stackIndex = transform.childCount-1;
		desiredPosition = (Vector3.down) * scoreCount;
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		theStack [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
	}


	public void MoveTile()
	{
		if (gameOver)
			return;
		tileTransition += Time.deltaTime * tileSpeed;
		if (isMovingOnX) 
		{
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		} else
		{
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransition) * BOUNDS_SIZE);
		}
	}

	public bool PlaceTile()
	{
		Transform t = theStack [stackIndex].transform;
		if (isMovingOnX) 
		{ 
			float deltaX = lastTilePosition.x - t.position.x;
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) 
			{
				// cut the tile
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if (stackBounds.x <= 0)
					return false;
				float middle = lastTilePosition.x + t.localPosition.x / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				t.localPosition = new Vector3 (middle- (lastTilePosition.x/2),scoreCount, lastTilePosition.z );
				CreateRubble
					(
					new Vector3((t.position.x > 0)
						? t.position.x +(t.localScale.x/2)
						: t.position.x -(t.localScale.x/2),
						t.position.y, 
						t.position.z),
					new Vector3 (Mathf.Abs(deltaX), 1, t.localScale.z)
						);
				t.localPosition =  new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
				
			}
			else 
			{
				
				if(combo > STACK_BOUNDS_GAIN)
				{
					stackBounds.x += STACK_BOUNDS_GAIN;
					if (stackBounds.x > BOUNDS_SIZE)
						stackBounds.x = BOUNDS_SIZE;
					float middle = lastTilePosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

		


					t.localPosition =  new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
					
					
				}
				combo++;
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
	
		}
		else
		{	
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {  
				// cut the tile
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0)
					return false;
				float middle = lastTilePosition.z + t.localPosition.z / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				CreateRubble
					(
						new Vector3(t.position.x,
					            t.position.y, 
					            (t.position.z > 0)
					            ? t.position.z +(t.localScale.z/2)
					            : t.position.z -(t.localScale.z/2)),
					new Vector3 (t.localScale.x, 1, Mathf.Abs(deltaZ))
						);
				t.localPosition = new Vector3 ( (lastTilePosition.x), scoreCount, middle - (lastTilePosition.z / 2));
				
			}
			else 
			{
				if(combo > STACK_BOUNDS_GAIN)
				{ 
					stackBounds.y += STACK_BOUNDS_GAIN;
					if (stackBounds.y > BOUNDS_SIZE)
						stackBounds.y = BOUNDS_SIZE;
					float middle = lastTilePosition.z + t.localPosition.z / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3 ( (lastTilePosition.x), scoreCount, middle - (lastTilePosition.z / 2));

				
				}
				combo++;
			
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}
		
		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
				:t.localPosition.z;
		
		isMovingOnX = !isMovingOnX;
		
		return true;
	}
	
	
	
	public void EndGame()
	{
		Debug.Log ("lose");
		gameOver = true;
		if (scoreCount >= maxScore) {
			maxScore = scoreCount;
			PlayerPrefs.SetInt ("maxScore", maxScore);

		}
		missionFailedPanel.SetActive (true);
		scoreText_1.text = "Current Score " + scoreCount.ToString ();
		maxScoreText.text = "Hightest Score " + maxScore.ToString();

		theStack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void CreateRubble(Vector3 pos, Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> (); 
		ColorMesh(go.GetComponent<MeshFilter>().mesh);

		go.GetComponent<MeshRenderer> ().material = stackMat;
	}


	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t )
	{
		if (t < 0.33f)
			return Color.Lerp (a, b , t/0.33f);
		else 	if (t < 0.66f)
			return Color.Lerp (b, c , (t-0.33f)/0.33f);
	//	if (t < 0.33f)
		else
			return Color.Lerp (c, d , (t- 0.66f)/0.66f);
		 
	}

	public void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount* 0.25f);

		for (int i= 0; i < vertices.Length; i++)
			colors [i] = Lerp4 (gameColors[0],gameColors[1],gameColors[2],gameColors[3], f);
			mesh.colors32 = colors;

	}

	public void Replay()
	{
		missionFailedPanel.SetActive (false);
		SceneManager.LoadScene (0);
	
	}

	
}

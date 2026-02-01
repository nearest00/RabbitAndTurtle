using UnityEngine;

public class N_222NoteSFX : MonoBehaviour
{
    private N_222SFXList sfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        sfx = Object.FindFirstObjectByType<N_222SFXList>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.RightArrow))
        {
            SoundManager.Instance.PlaySFX(sfx.NoteSound);
        }
    }
}

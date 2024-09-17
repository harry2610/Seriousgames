using UnityEngine;

public class Petting : DogInteractive
{
    protected override void OnDogHit(RaycastHit hitData)
    {
        this.transform.position = hitData.point;
    }

}

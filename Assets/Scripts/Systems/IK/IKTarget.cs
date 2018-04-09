using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget
{
    Vector3 _position = Vector3.zero;
    Quaternion _rotation = Quaternion.identity;

    float _positionweight = 0;
    float _rotationWeight = 0;

    public IKTarget()
    {
        _position = Vector3.zero;
        _rotation = Quaternion.identity;
        weight = 0;
    }
    public IKTarget(Vector3 position)
    {
        _position = position;
        _rotation = rotation;
        weight = 0;
    }
    public IKTarget(Quaternion rotation)
    {
        _position = position;
        _rotation = rotation;
        weight = 0;
    }
    public IKTarget(Vector3 position, float weight)
    {
        _position = position;
        _rotation = rotation;
        _positionweight = weight;
        _rotationWeight = 0;
    }
    public IKTarget(Quaternion rotation, float weight)
    {
        _position = position;
        _rotation = rotation;
        _positionweight = 0;
        _rotationWeight = weight;
    }
    public IKTarget(Vector3 position, Quaternion rotation, float weight)
    {
        _position = position;
        _rotation = rotation;
        this.weight = weight;
    }
    public IKTarget(Vector3 position, Quaternion rotation, float posWeight, float rotWeight)
    {
        _position = position;
        _rotation = rotation;
        _positionweight = posWeight;
        _rotationWeight = rotWeight;
    }

    public void Set(Transform t)
    {
        _position = t.position;
        _rotation = t.rotation;
    }

    public static IKTarget FromTransform(Transform t)
    {
        if (t)
            return new IKTarget(t.position, t.rotation, 0);
        else return null;
    }

    public Vector3 position
    {
        set { _position = value; }
        get { return _position; }
    }
    
    public Quaternion rotation
    {
        set { _rotation = value; }
        get { return _rotation; }
    }

    public float weight
    {
        get { return (_positionweight + _rotationWeight) / 2; }
        set
        {
            _positionweight = value;
            _rotationWeight = value;
        }
    }

    public float positionWeight
    {
        get { return _positionweight; }
        set { _positionweight = value; }
    }

    public float rotationWeight
    {
        get { return _rotationWeight; }
        set { _rotationWeight = value; }
    }
}

using System;

[Serializable]
public class Int2dArray
{
    public Int2dArray(int width, int height) {
        x = width;
        y = height;
        length = y * x;
        m = new int[y * x];
    }

    public int length;
    public int x, y;
    public int[] m;
    
    public int this[int index] {
        get { return m [index]; }
        set { m [index] = value; }
    }
    public int this[int x, int y] {
        get { return m [(this.y-1 -y) * this.x + x]; }
        set { m [(this.y-1 -y) * this.x + x] = value; }
    }
}
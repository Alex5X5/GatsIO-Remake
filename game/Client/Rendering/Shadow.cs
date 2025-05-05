namespace ShGame.game.Client.Rendering;
using System;
using System.Drawing;

using static ShGame.game.Client.Rendering.Renderer;

public class Shadow : Drawable {

    private readonly SupportsShadow attatch;

    public override void UpdateVertices() {

        //vertices[0]=(float)1;
        //vertices[1]=(float)Pos.y;
        //vertices[2]=0;
        //vertices[3]=(float)Pos.x+WIDTH;
        //vertices[4]=(float)Pos.y;
        //vertices[5]=0;
        //vertices[6]=(float)Pos.x+WIDTH;
        //vertices[7]=(float)Pos.y+HEIGHT;
        //vertices[8]=0;
        //      vertices[9]=(float)Pos.x;
        //vertices[10]=(float)Pos.y;
        //vertices[11]=0;
        //vertices[12]=(float)Pos.x;
        //vertices[13]=(float)Pos.y+HEIGHT;
        //vertices[14]=0;
        //vertices[15]=(float)Pos.x+WIDTH;
        //vertices[16]=(float)Pos.y+HEIGHT;
        //vertices[17]=0;
    }

    public Shadow(SupportsShadow attatch_) : base(18) {
        attatch = attatch_;
    }

    private static unsafe void CalculatePoints(Obstacle2* obstacle) {
        obstacle->WIDTH = obstacle->type switch {
            1 => 35,
            2 => 70,
            3 => 70,
            _ => 0,
        };
        obstacle->HEIGHT = obstacle->type switch {
            1 => 70,
            2 => 35,
            3 => 70,
            _ => 0,
        };
        obstacle->boundL.point1.Set(obstacle->Pos.x, obstacle->Pos.y, 0);
        obstacle->boundL.point2.Set(obstacle->Pos.x, obstacle->Pos.y+obstacle->HEIGHT, 0);
        obstacle->boundR.point1.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y, 0);
        obstacle->boundR.point2.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y+obstacle->HEIGHT, 0);
        obstacle->boundT.point1.Set(obstacle->boundT.point1);
        obstacle->boundT.point2.Set(obstacle->boundR.point1);
        obstacle->boundB.point1.Set(obstacle->boundL.point2);
        obstacle->boundB.point2.Set(obstacle->boundR.point2);
    }

    private unsafe Dir RelativeDir(Vector3d pos, Vector3d relativeTo) {
        Vector3d dir = relativeTo.Cpy().Sub(pos).Nor();
        if (dir.y > 1.0 / Math.Sqrt(2)) {
            return Dir.B;
        } else if (dir.y <= -1.0 / Math.Sqrt(2)) {
            return Dir.T;
        } else if (dir.x >= 1.0 / Math.Sqrt(2)) {
            return Dir.R;
        } else {
            return Dir.L;
        }
    }


    private unsafe void RenderObstacleShadows(Vector3d* v, Obstacle[]* l) {
        ArgumentNullException.ThrowIfNull(&v);
        //a list of points on the screen
        PointF[] points = new PointF[3];
        Vector3d ShadowPoint1 = new(0, 0, 0);
        Vector3d ShadowPoint2 = new(0, 0, 0);
        Vector3d ShadowPoint3 = new(0, 0, 0);
        Vector3d ShadowPoint4 = new(0, 0, 0);
        Vector3d[] sp;
        //return;
        foreach (Obstacle o in *l) {
            if (o==null)
                continue;
            o.GetShadowPoints(v, &ShadowPoint1, &ShadowPoint2);
            //sp=o.GetShadowPoints(v);
            if (!(v->x >= o.Pos.x && v->x <= o.Pos.x + o.WIDTH && v->y >= o.Pos.y && v->y <= o.Pos.y + o.HEIGHT)) {
                switch (RelativeDir(*v, o.Pos.Cpy().Add(new Vector3d(o.WIDTH / 2.0, o.HEIGHT / 2.0, 0)))) {
                    case Dir.T:
                        fixed (Line3d* line = &BORDER_TOP) {
                            ShadowPoint3 = ShadowHit(v, &ShadowPoint1, line);
                            ShadowPoint4 = ShadowHit(v, &ShadowPoint2, line);
                        }
                        break;
                    case Dir.B:
                        fixed (Line3d* line = &BORDER_BOTTOM) {
                            ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
                            ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
                        }
                        break;
                    case Dir.R:
                        fixed (Line3d* line = &BORDER_RIGHT) {
                            ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
                            ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
                        }
                        break;
                    case Dir.L:
                        fixed (Line3d* line = &BORDER_LEFT) {
                            ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
                            ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
                        }
                        break;
                        //default:
                        //	logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
                }
                //.fillPolygon(new int[] { (int)p1.x / gScl, (int)sp[0].x / gScl, (int)sp[1].x / gScl }, new int[] { (int)p1.y / gScl, (int)sp[0].y / gScl, (int)sp[1].y / gScl }, 3);

                //DrawShadowTriangle(ref points, ref p1.x, ref p1.y, ref p2.x, ref p2.y, ref sp[1].x, ref sp[1].y);
                //DrawShadowTriangle(ref points, ref p1.x, ref p1.y, ref sp[0].x, ref sp[0].y, ref sp[1].x, ref sp[1].y);
                //DrawShadowTriangle(ref points, ref ShadowPoint1.x, ref ShadowPoint1.y, ref ShadowPoint2.x, ref ShadowPoint2.y, ref ShadowPoint4.x, ref ShadowPoint4.y);
                //DrawShadowTriangle(ref points, ref ShadowPoint1.x, ref ShadowPoint1.y, ref ShadowPoint3.x, ref ShadowPoint3.y, ref ShadowPoint4.x, ref ShadowPoint4.y);
            }
        }
    }

    //	private bool pointOnField(Vector3d v) {
    //		return v.x>=0&&v.x<=UNSCALED_WIDTH&&v.y>=0&&v.y<=UNSCALED_HEIGHT;
    //	}

    private unsafe Vector3d ShadowHit(Vector3d* playerPosition, Vector3d* shadowPoint, Line3d* border) {
        //get the coordinates of the origin point of the border
        double oth1X = border->origin.x;
        double oth1Y = border->origin.y;
        double oth1Z = border->origin.z;
        //calculate the coordinates of a vector that starts at the origin point of the border
        //and points towards its second definition point
        Vector3d oth2 = border->origin.Cpy().Add(border->direction);
        double oth2X = oth2.x;
        double oth2Y = oth2.y;
        double oth2Z = oth2.z;
        //calculate a magical factor
        double u =
            (
                (oth1X - playerPosition->x) * (shadowPoint->y - playerPosition->y) -
                (oth1Y - playerPosition->y) * (shadowPoint->x - playerPosition->x)
            ) / (
                (oth2Y - oth1Y) * (shadowPoint->x - playerPosition->x) -
                (oth2X - oth1X) * (shadowPoint->y - playerPosition->y)
            );
        //magically merge the factor with the border
        return border->
            origin.
                Cpy().
                    Add(
                        oth2.
                            Sub(border->origin).
                                Scl(u)
                    );

        return new Vector3d(
            oth1X + u * (oth2X - oth1X),
            oth1Y + u * (oth2Y - oth1Y),
            oth1Z + u * (oth2Z - oth1Z));
    }
}
public interface SupportsShadow {
    public void GetShadowOrigins(out Vector3d point1, out Vector3d point2);
    public void GetPointOfView(out Vector3d point);
}
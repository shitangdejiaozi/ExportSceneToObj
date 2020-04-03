# ExportSceneToObj
Unity中导出场景为obj, 给Recastnavigation寻路使用
思考：
在实现项目中，不能直接将场景中的物体和地形mesh都导出， 对于大地形，面数会超过上限的。所以考虑用cube来代替关键的物体来做障碍物， 地形用原地形。

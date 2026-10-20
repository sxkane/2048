# 2048

这是一个使用 Unity 制作的 4x4 经典 2048 数字合成游戏。

## 项目环境

- Unity：6000.3.8f1
- 渲染管线：Universal Render Pipeline 2D
- 主要场景：`Assets/Scenes/SampleScene.unity`
- 核心脚本目录：`Assets/2048/Scripts`

## 游戏玩法

玩家通过方向键或 WASD 操作棋盘：

- 方向键 / WASD：移动方块
- 相同数字相撞时合并
- 每次有效移动后生成一个新的 `2`
- 右方向对应 x 轴正方向
- 上方向对应 y 轴正方向
- 点击 Restart 重新开始游戏

当前输入还支持 Unity Input System 中配置的手柄和摇杆输入。

## 核心规则

游戏棋盘使用 `(x, y)` 坐标表示，其中：

- x 增大表示向右
- y 增大表示向上
- 棋盘数据保存在 `GameCore` 中
- 视图通过 `BoardView` 将逻辑坐标映射到场景中的格子位置

胜利条件为棋盘填满 16 个格子，并且包含从 `2^1` 到 `2^16` 的全部数字：

```text
2, 4, 8, 16, ..., 65536
```

当棋盘填满且不存在可合并的相邻方块时，发布游戏结束事件。

## 项目结构

```text
Assets/2048/Scripts
├── Core
│   └── GameCore.cs              # 棋盘数据、移动、合并和胜负判断
├── Events
│   ├── EventBus.cs              # 全局事件总线
│   ├── TileMoveEvent.cs         # 方块移动事件
│   ├── TileGenerateEvent.cs     # 新方块生成事件
│   ├── GameRestartEvent.cs      # 重启事件
│   ├── GameEndEvent.cs          # 游戏结束事件
│   └── GameSuccessEvent.cs      # 游戏成功事件
├── Input
│   └── InputHandler.cs          # 输入读取和方向转换
├── Values
│   ├── Direction.cs             # 移动方向
│   └── TileMove.cs              # 方块移动数据
└── Views
    ├── BoardView.cs             # 棋盘方块显示和移动动画
    ├── TileView.cs              # 单个方块视图和动画
    ├── ScoreView.cs             # 分数显示
    ├── GameEndView.cs           # 结束界面
    └── ViewManager.cs           # UI 事件响应
```

## 事件流程

核心逻辑和 UI 通过事件解耦：

1. `InputHandler` 读取输入并转换为 `Direction`。
2. `GameManager` 调用 `GameCore.Move()`。
3. `GameCore` 发布移动、生成、分数和胜负事件。
4. `ViewManager` 接收事件并更新棋盘、分数和结束界面。
5. `BoardView` 按事件顺序播放动画，避免快速输入造成视图状态错乱。

## 构建检查

可以使用以下命令检查 C# 工程是否能够构建：

```powershell
dotnet build 2048.sln --no-restore
```

实际运行和场景验证建议使用 Unity Editor 打开项目并运行 `SampleScene`。

## 写在最后

作者：sxkane

时间：2026/10/20

本来是想要每天复刻一个小游戏，将2048这款游戏作为最开始的作品，在进行代码编写的时候反而暴露了很多问题，
导致原本想要3h完成的工作，拖沓了半天左右。
一些反思：
1. 需要先构思完架构在进行代码的编写
2. 完成的是快速原型，需要进行功能的取舍
3. 感觉自己还是不行，需要巩固一些知识
希望在每天的项目中改变这些问题
# XWopcUA 测试指南

## 测试服务器说明

我已经为您创建了一个OPC UA测试服务器，用于测试XWopcUA客户端的功能。

### 测试服务器特性

1. **服务器地址**: `opc.tcp://localhost:4840`

2. **支持的安全模式**:
   - None (无加密)
   - Sign (签名)
   - SignAndEncrypt (签名和加密)

3. **测试变量**:
   - **Temperature** (Double): 模拟温度值，范围 15-25°C，每秒更新
   - **Pressure** (Double): 模拟压力值，范围 91-111 kPa，每秒更新
   - **Status** (Boolean): 系统状态，每10秒切换一次
   - **Counter** (Int32): 递增计数器，每秒增加1
   - **ArrayData** (Double[]): 静态数组 [1.1, 2.2, 3.3, 4.4, 5.5]
   - **Message** (String): 静态消息 "Hello from XWopcUA Test Server!"

4. **测试方法**:
   - **Multiply**: 接受两个Double参数，返回乘积

## 使用步骤

### 1. 编译和运行测试服务器

1. 在Visual Studio中打开解决方案
2. 右键点击 TestServer 项目 → "还原 NuGet 包"
3. 编译整个解决方案 (F6)
4. 右键点击 TestServer 项目 → "调试" → "启动新实例"
5. 看到控制台显示 "Server is running at: opc.tcp://localhost:4840"

### 2. 运行客户端测试

1. 保持测试服务器运行
2. 右键点击 XWopcUA 项目 → "调试" → "启动新实例"
3. 在客户端界面进行以下测试

### 3. 测试场景

#### A. 无加密连接测试
1. Server URL: `opc.tcp://localhost:4840`
2. Security Mode: None
3. 点击 Connect
4. 应该显示 "Connected"

#### B. 加密连接测试
1. Server URL: `opc.tcp://localhost:4840`
2. Security Mode: SignAndEncrypt
3. Security Policy: Basic256Sha256
4. 可能需要处理证书信任（服务器配置为自动接受）
5. 点击 Connect

#### C. 节点浏览测试
1. 连接成功后，点击 "Browse..."
2. 展开节点树：Server → Objects → TestData
3. 应该看到所有测试变量

#### D. 读取测试
1. 在节点浏览器中选择 Temperature
2. 点击 Select
3. 在主界面点击 Read
4. 应该看到当前温度值（15-25之间）
5. 多次点击Read，值应该不断变化

#### E. 写入测试
1. Node ID: `ns=2;s=Message`
2. Value: 输入新的消息文本
3. 点击 Write
4. 再次Read验证写入成功

#### F. 订阅测试（如果实现了UI）
1. 选择 Temperature 或 Counter 节点
2. 创建订阅
3. 应该能看到值的实时更新

## 故障排除

### 端口被占用
如果4840端口被占用，修改TestServer代码中的端口号：
```csharp
config.ServerConfiguration.BaseAddresses.Add("opc.tcp://localhost:4841");
```

### 证书问题
测试服务器配置为自动接受不受信任的证书。如果仍有问题：
1. 删除 TestServer\bin\Debug\Certificates 文件夹
2. 重新运行服务器，会自动生成新证书

### 连接失败
1. 确保TestServer正在运行
2. 检查Windows防火墙是否阻止了连接
3. 尝试使用 `opc.tcp://127.0.0.1:4840` 代替 localhost

## 高级测试

### 方法调用测试
使用OPC UA专业工具（如UaExpert）测试Multiply方法：
- 输入参数: a=3.0, b=4.0
- 预期结果: 12.0

### 性能测试
- Counter变量每秒更新，可用于测试订阅性能
- 同时订阅多个变量测试客户端处理能力

## 注意事项

1. 测试服务器仅用于开发测试，不适合生产环境
2. 服务器配置为自动接受所有证书，实际应用中应该严格验证
3. 用户认证未实现，始终使用匿名连接
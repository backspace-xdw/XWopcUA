# OPC UA包升级说明

## 升级内容

已将OPC Foundation包从已弃用的1.4.371.86版本升级到最新的1.5.374.126版本。

### 主要变更：

1. **包版本更新** (packages.config)
   - OPCFoundation.NetStandard.Opc.Ua: 1.4.371.86 → 1.5.374.126
   - OPCFoundation.NetStandard.Opc.Ua.Client: 1.4.371.86 → 1.5.374.126
   - OPCFoundation.NetStandard.Opc.Ua.Core: 1.4.371.86 → 1.5.374.126
   - OPCFoundation.NetStandard.Opc.Ua.Configuration: 1.4.371.86 → 1.5.374.126
   - OPCFoundation.NetStandard.Opc.Ua.Security.Certificates: 1.4.371.86 → 1.5.374.126

2. **新增依赖包**
   - Newtonsoft.Json 13.0.3
   - System.Runtime.CompilerServices.Unsafe 6.0.0

3. **代码更新**
   - CertificateService.cs: 更新了CreateSelfSignedCertificate方法，使用新的CertificateFactory API
   - DataService.cs: 添加了System.Linq引用

## 在Visual Studio中操作步骤

1. **清理旧包**
   ```
   - 关闭Visual Studio
   - 删除packages文件夹
   - 删除.vs文件夹（如果存在）
   ```

2. **还原新包**
   ```
   - 打开XWopcUA.sln
   - 右键解决方案 → "还原 NuGet 包"
   - 或者：工具 → NuGet 包管理器 → 程序包管理器控制台
   - 执行：Update-Package -reinstall
   ```

3. **重新生成项目**
   ```
   - 生成 → 清理解决方案
   - 生成 → 重新生成解决方案
   ```

## 注意事项

1. 新版本的OPC UA SDK包含了一些安全性和性能改进
2. API基本保持向后兼容，只有少量变更
3. 如果遇到编译错误，请检查是否所有NuGet包都正确还原

## 测试建议

升级后建议测试以下功能：
- 基本连接（加密/非加密）
- 证书创建和管理
- 节点浏览和数据读写
- 订阅功能

## 回滚方法

如果需要回滚到旧版本：
1. 将packages.config中的版本号改回1.4.371.86
2. 将CertificateService.cs中的CreateSelfSignedCertificate方法改回原始代码
3. 重新还原NuGet包
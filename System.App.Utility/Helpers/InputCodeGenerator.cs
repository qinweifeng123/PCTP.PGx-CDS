﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.App.Utility.Helpers;
//using System.Text.RegularExpressions;
//using System.App.Model.Entities;
//using System.Web.Script.Serialization;

//namespace System.App.Utility.Helpers
//{
//    /// <summary>
//    /// Generate input code
//    /// </summary>
//    public static class InputCodeGenerator
//    {
//        /// <summary>
//        /// Generate input code for text in single language. e.g. Pinyin for Chinese or acronym for English
//        /// </summary>
//        /// <param name="text">input text</param>
//        /// <param name="language">dialect or language</param>
//        /// <returns>input code</returns>
//        public static string GetInputCodeForSingleLanguage(string text, string language = TermTranslationHelper.ENGLISH)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return string.Empty;

//            switch (language)
//            {
//                case TermTranslationHelper.ENGLISH:
//                    return text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(0, 1).ToLower()).Aggregate((x, y) => x + y);
//                case TermTranslationHelper.CHINESE_SIMPLIFIED:
//                    return text.Select(x => Dict_Pinyin.FirstOrDefault(y => y.Character == x.ToString()).Pinyin.ElementAt(0).ToString().ToLower()).Aggregate((x, y) => x + y);
//            }
//            return text;
//        }

//        /// <summary>
//        /// Generate input code for text. e.g. Pinyin for Chinese or acronym for English
//        /// </summary>
//        /// <param name="text">a mixture of text in different languanges</param>
//        /// <returns>input code</returns>
//        public static string GetInputCode(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return string.Empty;

//            var ret = string.Empty;

//            var regex = new Regex(@"\[([^]]*)\]|\(([^]]*)\)|（([^]]*)）");
//            // neglect string inside parentheses or brackets
//            foreach (var x in regex.Matches(text))
//            {
//                text = text.Replace(x.ToString(), string.Empty);
//            }

//            // if the string is a single English word, return as it is
//            if (Regex.IsMatch(text, "^[a-zA-Z0-9]*$"))
//                return text.ToLower();

//            // if the string is pure English, 
//            if (Regex.IsMatch(text.Replace(" ", string.Empty), "^[a-zA-Z0-9]*$"))
//                return GetInputCodeForSingleLanguage(text, TermTranslationHelper.ENGLISH);

//            text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x =>
//            {
//                if (Regex.IsMatch(x, "^[a-zA-Z0-9]*$"))
//                {
//                    ret += x.ToLower();
//                }
//                else
//                {
//                    foreach (var y in x)
//                    {
//                        // skip if character is control, symbol, punctuation, separator, or surrogate
//                        if (char.IsLetter(y) || char.IsDigit(y))
//                        {
//                            // Some characters are polyphonic. We use the first/last pinyin. This brings potential error, but acceptable. 
//                            // e.g. 重症 -> cz, but should be zz. 骨折 -> gs, but should be gz
//                            var pinyin = Dict_Pinyin.LastOrDefault(z => z.Character == y.ToString());
//                            if (pinyin != null && string.IsNullOrWhiteSpace(pinyin.Pinyin) == false)
//                                ret += pinyin.Pinyin.ElementAt(0).ToString().ToLower();
//                            else if (Regex.IsMatch(y.ToString(), "^[a-zA-Z0-9]*$"))
//                                ret += y.ToString().ToLower();

//                        }
//                    }
//                }
//            });
//            return ret;
//        }

//        private static List<Dict_Pinyin> dict_pinyin = null;
//        public static List<Dict_Pinyin> Dict_Pinyin
//        {

//            get
//            {
//                if (dict_pinyin == null)
//                {
//                    dict_pinyin = new List<Dict_Pinyin>();
//                    var obj = new JavaScriptSerializer().Deserialize<List<List<string>>>(System.App.Utility.Properties.Resources.Pinyin_cn);

//                    obj.ForEach(x =>
//                    {
//                        dict_pinyin.Add(new Dict_Pinyin()
//                        {
//                            Character = x.ElementAt(0),
//                            Pinyin = x.ElementAt(1),
//                            GB = x.ElementAt(2),
//                            Polyphone = bool.Parse(x.ElementAt(3))
//                        });
//                    });
//                }
//                return dict_pinyin;
//            }
//        }

//#if false
//        // only need run once. the pinyin table now has been put into resx.

//        public void SeedPinyinDict()
//        {
//            var pinyins = new List<Dict_Pinyin>();

//            var lines = InputCodeGenerator.pinyin_table.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (var line in lines)
//            {
//                if (string.IsNullOrWhiteSpace(line))
//                    continue;

//                var fields = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
//                if (fields.Count() == 3)
//                {
//                    var pinyin = new Dict_Pinyin()
//                    {
//                        Character = fields.ElementAt(2).Substring(0, 1),
//                        GB = fields.ElementAt(1),
//                        Pinyin = fields.ElementAt(0),
//                        Polyphone = fields.ElementAt(2).EndsWith("*")
//                    };
//                    pinyins.Add(pinyin);
//                }
//                else
//                {
//                    System.Diagnostics.Debug.Assert(false);
//                }
//            }

//            var s = new JavaScriptSerializer().Serialize(pinyins.Select(x => new[] { x.Character, x.Pinyin, x.GB, x.Polyphone.ToString() }).ToList());
//            AddOrUpdateCodingSystem(ConstCodingSystem.PINYIN_CN, s);
//        }

//        public const string pinyin_table = @"
//a1      B0A1 啊*

//a1      B0A2 阿*

//a1      DFB9 吖

//a1      EBE7 腌*

//a1      EFB9 锕

//a2      B0A1 啊*

//a2      E0C4 嗄*

//a3      B0A1 啊*

//a4      B0A1 啊*

//a5      B0A1 啊*

//a5      BAC7 呵*

//ai1     B0A3 埃

//ai1     B0A4 挨*

//ai1     B0A5 哎*

//ai1     B0A6 唉*

//ai1     B0A7 哀

//ai1     E0C8 嗳*

//ai1     EFCD 锿

//ai2     B0A4 挨*

//ai2     B0A8 皑

//ai2     B0A9 癌

//ai2     DEDF 捱

//ai3     B0AA 蔼

//ai3     B0AB 矮

//ai3     DAC0 诶*

//ai3     E0C8 嗳*

//ai3     F6B0 霭

//ai4     B0A6 唉*

//ai4     B0AC 艾*

//ai4     B0AD 碍

//ai4     B0AE 爱

//ai4     B0AF 隘

//ai4     E0C8 嗳*

//ai4     E0C9 嗌*

//ai4     E6C8 嫒

//ai4     E8A8 瑷

//ai4     EAD3 暧

//ai4     EDC1 砹

//an1     B0B0 鞍

//an1     B0B1 氨

//an1     B0B2 安

//an1     B3A7 厂*

//an1     B9E3 广*

//an1     DACF 谙

//an1     E2D6 庵

//an1     E8F1 桉

//an1     F0C6 鹌

//an3     B0B3 俺

//an3     DBFB 埯

//an3     DEEE 揞

//an3     EFA7 铵

//an4     B0B4 按

//an4     B0B5 暗

//an4     B0B6 岸

//an4     B0B7 胺

//an4     B0B8 案

//an4     E1ED 犴*

//an4     F7F6 黯

//ang1    B0B9 肮

//ang2    B0BA 昂

//ang4    B0BB 盎

//ao1     B0BC 凹

//ao1     B0BE 熬*

//ao2     B0BD 敖

//ao2     B0BE 熬*

//ao2     B0BF 翱

//ao2     E0BB 嗷

//ao2     E2DA 廒

//ao2     E5DB 遨

//ao2     E9E1 獒

//ao2     F1FA 聱

//ao2     F2FC 螯

//ao2     F7A1 鳌

//ao2     F7E9 鏖

//ao3     B0C0 袄

//ao3     DED6 拗*

//ao3     E6C1 媪

//ao4     B0C1 傲

//ao4     B0C2 奥

//ao4     B0C3 懊

//ao4     B0C4 澳

//ao4     DBEA 坳

//ao4     DED6 拗*

//ao4     E1AE 岙

//ao4     E6F1 骜

//ao4     F6CB 鏊

//ba1     B0C5 芭

//ba1     B0C6 捌

//ba1     B0C7 扒*

//ba1     B0C8 叭

//ba1     B0C9 吧*

//ba1     B0CA 笆

//ba1     B0CB 八

//ba1     B0CC 疤

//ba1     B0CD 巴

//ba1     E1B1 岜

//ba1     F4CE 粑

//ba2     B0CE 拔

//ba2     B0CF 跋

//ba2     DCD8 茇

//ba2     DDC3 菝

//ba2     F7C9 魃

//ba3     B0D0 靶

//ba3     B0D1 把*

//ba3     EED9 钯*

//ba4     B0D1 把*

//ba4     B0D2 耙*

//ba4     B0D3 坝

//ba4     B0D4 霸

//ba4     B0D5 罢*

//ba4     B0D6 爸

//ba4     E5B1 灞

//ba4     F6D1 鲅

//ba5     B0C9 吧*

//ba5     B0D5 罢*

//bai1    EAFE 掰

//bai1    EBA2 擘*

//bai2    B0D7 白

//bai3    B0D8 柏*

//bai3    B0D9 百

//bai3    B0DA 摆

//bai3    B0DB 佰

//bai3    B2AE 伯*

//bai3    DEE3 捭

//bai4    B0DC 败

//bai4    B0DD 拜

//bai4    B0DE 稗

//bai4    DFC2 呗*

//ban1    B0DF 斑

//ban1    B0E0 班

//ban1    B0E1 搬

//ban1    B0E2 扳

//ban1    B0E3 般

//ban1    B0E4 颁

//ban1    F1A3 瘢

//ban1    F1AD 癍

//ban3    B0E5 板

//ban3    B0E6 版

//ban3    DAE6 阪

//ban3    DBE0 坂

//ban3    EED3 钣

//ban3    F4B2 舨

//ban4    B0E7 扮

//ban4    B0E8 拌

//ban4    B0E9 伴

//ban4    B0EA 瓣

//ban4    B0EB 半

//ban4    B0EC 办

//ban4    B0ED 绊

//bang1   B0EE 邦

//bang1   B0EF 帮

//bang1   B0F0 梆

//bang1   E4BA 浜

//bang3   B0F1 榜

//bang3   B0F2 膀*

//bang3   B0F3 绑

//bang4   B0F4 棒

//bang4   B0F5 磅*

//bang4   B0F6 蚌*

//bang4   B0F7 镑

//bang4   B0F8 傍

//bang4   B0F9 谤

//bang4   DDF2 蒡

//bao1    B0FA 苞

//bao1    B0FB 胞

//bao1    B0FC 包

//bao1    B0FD 褒

//bao1    B0FE 剥*

//bao1    C5DA 炮*

//bao1    E6DF 孢

//bao1    ECD2 煲

//bao1    F6B5 龅

//bao2    B1A1 薄*

//bao2    B1A2 雹

//bao3    B1A3 保

//bao3    B1A4 堡*

//bao3    B1A5 饱

//bao3    B1A6 宝

//bao3    DDE1 葆

//bao3    F0B1 鸨

//bao3    F1D9 褓

//bao4    B1A7 抱

//bao4    B1A8 报

//bao4    B1A9 暴*

//bao4    B1AA 豹

//bao4    B1AB 鲍

//bao4    B1AC 爆

//bao4    C5D9 刨*

//bao4    C6D8 曝*

//bao4    C6D9 瀑*

//bao4    F5C0 趵

//bei1    B1AD 杯

//bei1    B1AE 碑

//bei1    B1AF 悲

//bei1    B1B0 卑

//bei1    B1B3 背*

//bei1    DAE9 陂*

//bei1    E2D8 庳*

//bei1    F0C7 鹎

//bei3    B1B1 北

//bei4    B1B2 辈

//bei4    B1B3 背*

//bei4    B1B4 贝

//bei4    B1B5 钡

//bei4    B1B6 倍

//bei4    B1B7 狈

//bei4    B1B8 备

//bei4    B1B9 惫

//bei4    B1BA 焙

//bei4    B1BB 被

//bei4    D8C3 孛

//bei4    DAFD 邶

//bei4    DDED 蓓

//bei4    E3A3 悖

//bei4    EDD5 碚

//bei4    F1D8 褙

//bei4    F6CD 鐾

//bei4    F7B9 鞴

//bei5    B1DB 臂*

//bei5    DFC2 呗*

//ben1    B1BC 奔*

//ben1    EADA 贲*

//ben1    EFBC 锛

//ben3    B1BD 苯

//ben3    B1BE 本

//ben3    DBCE 畚

//ben4    B1BC 奔*

//ben4    B1BF 笨

//ben4    BABB 夯*

//ben4    DBD0 坌

//beng1   B1C0 崩

//beng1   B1C1 绷*

//beng1   E0D4 嘣

//beng2   B1C2 甭

//beng3   B1C1 绷*

//beng4   B0F6 蚌*

//beng4   B1C3 泵

//beng4   B1C4 蹦

//beng4   B1C5 迸

//beng4   EAB4 甏

//bi1     B1C6 逼

//bi2     B1C7 鼻

//bi2     DDA9 荸

//bi3     B1C8 比

//bi3     B1C9 鄙

//bi3     B1CA 笔

//bi3     B1CB 彼

//bi3     D8B0 匕

//bi3     D9C2 俾

//bi3     DFC1 吡*

//bi3     E5FE 妣

//bi3     EFF5 秕

//bi3     F4B0 舭

//bi4     B1CC 碧

//bi4     B1CD 蓖

//bi4     B1CE 蔽

//bi4     B1CF 毕

//bi4     B1D0 毙

//bi4     B1D1 毖

//bi4     B1D2 币

//bi4     B1D3 庇

//bi4     B1D4 痹

//bi4     B1D5 闭

//bi4     B1D6 敝

//bi4     B1D7 弊

//bi4     B1D8 必

//bi4     B1D9 辟*

//bi4     B1DA 壁

//bi4     B1DB 臂*

//bi4     B1DC 避

//bi4     B1DD 陛

//bi4     C3D8 秘*

//bi4     C3DA 泌*

//bi4     DCEA 荜

//bi4     DDC9 萆

//bi4     DEB5 薜

//bi4     DFD9 哔

//bi4     E1F9 狴

//bi4     E2D8 庳*

//bi4     E3B9 愎

//bi4     E4E4 滗

//bi4     E5A8 濞

//bi4     E5F6 弼

//bi4     E6BE 婢

//bi4     E6D4 嬖

//bi4     E8B5 璧

//bi4     EADA 贲*

//bi4     EEA2 睥*

//bi4     EEAF 畀

//bi4     EEE9 铋

//bi4     F1D4 裨*

//bi4     F3D9 筚

//bi4     F3EB 箅

//bi4     F3F7 篦

//bi4     F4C5 襞

//bi4     F5CF 跸

//bi4     F7C2 髀

//bian1   B1DE 鞭

//bian1   B1DF 边

//bian1   B1E0 编

//bian1   ECD4 煸

//bian1   EDBE 砭

//bian1   F2F9 蝙

//bian1   F3D6 笾

//bian1   F6FD 鳊

//bian3   B1E1 贬

//bian3   B1E2 扁*

//bian3   D8D2 匾

//bian3   EDDC 碥

//bian3   F1B9 窆

//bian3   F1DB 褊

//bian4   B1E3 便*

//bian4   B1E4 变

//bian4   B1E5 卞

//bian4   B1E6 辨

//bian4   B1E7 辩

//bian4   B1E8 辫

//bian4   B1E9 遍

//bian4   DBCD 弁

//bian4   DCD0 苄

//bian4   E2ED 忭

//bian4   E3EA 汴

//bian4   E7C2 缏*

//biao1   B1EA 标

//biao1   B1EB 彪

//biao1   B1EC 膘

//biao1   E6F4 骠*

//biao1   E8BC 杓*

//biao1   ECA9 飑

//biao1   ECAD 飙

//biao1   ECAE 飚

//biao1   EFDA 镖

//biao1   EFF0 镳

//biao1   F1A6 瘭

//biao1   F7D4 髟

//biao3   B1ED 表

//biao3   E6BB 婊

//biao3   F1D1 裱

//biao4   F7A7 鳔

//bie1    B1EE 鳖

//bie1    B1EF 憋

//bie1    B1F1 瘪*

//bie2    B1F0 别*

//bie2    F5BF 蹩

//bie3    B1F1 瘪*

//bie4    B1F0 别*

//bin1    B1F2 彬

//bin1    B1F3 斌

//bin1    B1F4 濒

//bin1    B1F5 滨

//bin1    B1F6 宾

//bin1    D9CF 傧

//bin1    E1D9 豳

//bin1    E7CD 缤

//bin1    E7E3 玢*

//bin1    E9C4 槟*

//bin1    EFD9 镔

//bin4    B1F7 摈

//bin4    E9EB 殡

//bin4    EBF7 膑

//bin4    F7C6 髌

//bin4    F7DE 鬓

//bing1   B1F8 兵

//bing1   B1F9 冰

//bing1   B2A2 并*

//bing1   E9C4 槟*

//bing3   B1FA 柄*

//bing3   B1FB 丙

//bing3   B1FC 秉

//bing3   B1FD 饼

//bing3   B1FE 炳

//bing3   C6C1 屏*

//bing3   D9F7 禀

//bing3   DAFB 邴

//bing4   B1FA 柄*

//bing4   B2A1 病

//bing4   B2A2 并*

//bing4   DEF0 摒

//bo1     B0FE 剥*

//bo1     B2A3 玻

//bo1     B2A4 菠

//bo1     B2A5 播

//bo1     B2A6 拨

//bo1     B2A7 钵

//bo1     B2A8 波

//bo1     E2C4 饽

//bo2     B0D8 柏*

//bo2     B1A1 薄*

//bo2     B2A9 博

//bo2     B2AA 勃

//bo2     B2AB 搏

//bo2     B2AC 铂

//bo2     B2AD 箔

//bo2     B2AE 伯*

//bo2     B2AF 帛

//bo2     B2B0 舶

//bo2     B2B1 脖

//bo2     B2B2 膊

//bo2     B2B3 渤

//bo2     B2B4 泊*

//bo2     B2B5 驳

//bo2     C6C7 魄*

//bo2     D9F1 亳

//bo2     EDE7 礴

//bo2     EEE0 钹

//bo2     F0BE 鹁

//bo2     F5DB 踣

//bo3     F4A4 簸*

//bo3     F5CB 跛

//bo4     B0D8 柏*

//bo4     B1A1 薄*

//bo4     E9DE 檗

//bo4     EBA2 擘*

//bo4     F4A4 簸*

//bo5     B2B7 卜*

//bo5     E0A3 啵

//bu1     E5CD 逋

//bu1     EACE 晡

//bu1     EEDF 钸

//bu2     B2BB 不*

//bu2     F5B3 醭

//bu3     B1A4 堡*

//bu3     B2B6 捕

//bu3     B2B7 卜*

//bu3     B2B8 哺

//bu3     B2B9 补

//bu3     DFB2 卟

//bu4     B2BA 埠

//bu4     B2BB 不*

//bu4     B2BC 布

//bu4     B2BD 步

//bu4     B2BE 簿

//bu4     B2BF 部

//bu4     B2C0 怖

//bu4     C6D2 埔*

//bu4     EAB3 瓿

//bu4     EED0 钚

//ca1     B2C1 擦

//ca1     B2F0 拆*

//ca1     E0EA 嚓*

//ca3     EDE5 礤

//cai1    B2C2 猜

//cai2    B2C3 裁

//cai2    B2C4 材

//cai2    B2C5 才

//cai2    B2C6 财

//cai3    B2C7 睬

//cai3    B2C8 踩

//cai3    B2C9 采*

//cai3    B2CA 彩

//cai4    B2C9 采*

//cai4    B2CB 菜

//cai4    B2CC 蔡

//can1    B2CD 餐

//can1    B2CE 参*

//can1    E6EE 骖

//can2    B2CF 蚕

//can2    B2D0 残

//can2    B2D1 惭

//can3    B2D2 惨

//can3    F7F5 黪

//can4    B2D3 灿

//can4    B2F4 掺*

//can4    E5EE 孱*

//can4    E8B2 璨

//can4    F4D3 粲

//cang1   B2D4 苍

//cang1   B2D5 舱

//cang1   B2D6 仓

//cang1   B2D7 沧

//cang1   D8F7 伧

//cang2   B2D8 藏*

//cao1    B2D9 操

//cao1    B2DA 糙

//cao2    B2DB 槽

//cao2    B2DC 曹

//cao2    E0D0 嘈

//cao2    E4EE 漕

//cao2    F3A9 螬

//cao2    F4BD 艚

//cao3    B2DD 草

//ce4     B2DE 厕

//ce4     B2DF 策

//ce4     B2E0 侧*

//ce4     B2E1 册

//ce4     B2E2 测

//ce4     E2FC 恻

//cen1    B2CE 参*

//cen2    E1AF 岑

//cen2    E4B9 涔

//ceng1   E0E1 噌

//ceng2   B2E3 层

//ceng2   D4F8 曾*

//ceng4   B2E4 蹭

//cha1    B2E5 插

//cha1    B2E6 叉*

//cha1    B2EA 碴*

//cha1    B2EE 差*

//cha1    D4FB 喳*

//cha1    E0EA 嚓*

//cha1    E2C7 馇

//cha1    E8BE 杈*

//cha1    EFCA 锸

//cha2    B2E6 叉*

//cha2    B2E7 茬*

//cha2    B2E8 茶

//cha2    B2E9 查*

//cha2    B2EA 碴*

//cha2    B2EB 搽

//cha2    B2EC 察

//cha2    E2AA 猹

//cha2    E9AB 楂

//cha2    E9B6 槎

//cha2    E9DF 檫

//cha3    B2E6 叉*

//cha3    EFEF 镲

//cha3    F1C3 衩*

//cha4    B2ED 岔

//cha4    B2EE 差*

//cha4    B2EF 诧

//cha4    C9B2 刹*

//cha4    E3E2 汊

//cha4    E6B1 姹

//cha4    E8BE 杈*

//cha4    F1C3 衩*

//chai1   B2EE 差*

//chai1   B2F0 拆*

//chai1   EECE 钗

//chai2   B2F1 柴

//chai2   B2F2 豺

//chai2   D9AD 侪

//chai4   F0FB 瘥*

//chai4   F2B2 虿

//chan1   B2F3 搀

//chan1   B2F4 掺*

//chan1   EAE8 觇

//chan2   B2F5 蝉

//chan2   B2F6 馋

//chan2   B2F7 谗

//chan2   B2F8 缠

//chan2   B5A5 单*

//chan2   E2DC 廛

//chan2   E4FD 潺

//chan2   E5A4 澶

//chan2   E5EE 孱*

//chan2   E6BF 婵

//chan2   ECF8 禅*

//chan2   EFE2 镡*

//chan2   F3B8 蟾

//chan2   F5F0 躔

//chan3   B2F9 铲

//chan3   B2FA 产

//chan3   B2FB 阐

//chan3   D9E6 冁

//chan3   DAC6 谄

//chan3   DDDB 蒇

//chan3   E6F6 骣

//chan4   B2FC 颤*

//chan4   E2E3 忏

//chan4   E5F1 羼

//chang1  B2FD 昌

//chang1  B2FE 猖

//chang1  D8F6 伥*

//chang1  DDC5 菖

//chang1  E3D1 阊

//chang1  E6BD 娼

//chang1  F6F0 鲳

//chang2  B3A1 场*

//chang2  B3A2 尝

//chang2  B3A3 常

//chang2  B3A4 长*

//chang2  B3A5 偿

//chang2  B3A6 肠

//chang2  C9D1 裳*

//chang2  CCC8 倘

//chang2  DCC9 苌

//chang2  E1E4 徜

//chang2  E6CF 嫦

//chang3  B3A1 场*

//chang3  B3A7 厂*

//chang3  B3A8 敞*

//chang3  E3AE 惝

//chang3  EAC6 昶

//chang3  EBA9 氅

//chang4  B3A9 畅

//chang4  B3AA 唱

//chang4  B3AB 倡

//chang4  DBCB 鬯

//chang4  E2EA 怅

//chao1   B3AC 超

//chao1   B3AD 抄

//chao1   B3AE 钞

//chao1   B3B3 吵*

//chao1   B4C2 绰*

//chao1   BDCB 剿*

//chao1   E2F7 怊

//chao1   ECCC 焯*

//chao2   B3AF 朝*

//chao2   B3B0 嘲*

//chao2   B3B1 潮

//chao2   B3B2 巢

//chao2   EACB 晁

//chao3   B3B3 吵*

//chao3   B3B4 炒

//chao4   F1E9 耖

//che1    B3B5 车*

//che1    EDBA 砗

//che3    B3B6 扯

//che3    B3DF 尺*

//che4    B3B7 撤

//che4    B3B8 掣

//che4    B3B9 彻

//che4    B3BA 澈

//che4    DBE5 坼

//chen1   B3BB 郴

//chen1   DED3 抻

//chen1   E0C1 嗔

//chen1   E8A1 琛

//chen2   B3BC 臣

//chen2   B3BD 辰

//chen2   B3BE 尘

//chen2   B3BF 晨

//chen2   B3C0 忱

//chen2   B3C1 沉

//chen2   B3C2 陈

//chen2   C9F2 沈*

//chen2   DAC8 谌

//chen2   E5B7 宸

//chen3   EDD7 碜

//chen4   B3C3 趁

//chen4   B3C4 衬

//chen4   B3C6 称*

//chen4   DADF 谶

//chen4   E9B4 榇

//chen4   F6B3 龀

//cheng1  B3C5 撑

//cheng1  B3C6 称*

//cheng1  E8DF 柽

//cheng1  EEAA 瞠

//cheng1  EEF5 铛*

//cheng1  F2C9 蛏

//cheng2  B3C7 城

//cheng2  B3C8 橙

//cheng2  B3C9 成

//cheng2  B3CA 呈

//cheng2  B3CB 乘*

//cheng2  B3CC 程

//cheng2  B3CD 惩

//cheng2  B3CE 澄*

//cheng2  B3CF 诚

//cheng2  B3D0 承

//cheng2  CAA2 盛*

//cheng2  D8A9 丞

//cheng2  DBF4 埕

//cheng2  E8C7 枨

//cheng2  EBF3 塍

//cheng2  EEF1 铖

//cheng2  F1CE 裎

//cheng2  F5A8 酲

//cheng3  B3D1 逞

//cheng3  B3D2 骋

//cheng4  B3C6 称*

//cheng4  B3D3 秤

//chi1    B3D4 吃

//chi1    B3D5 痴

//chi1    DFEA 哧

//chi1    E0CD 嗤

//chi1    E6CA 媸

//chi1    EDF7 眵

//chi1    F0B7 鸱

//chi1    F2BF 蚩

//chi1    F3A4 螭

//chi1    F3D7 笞

//chi1    F7CE 魑

//chi2    B3D6 持

//chi2    B3D7 匙*

//chi2    B3D8 池

//chi2    B3D9 迟

//chi2    B3DA 弛

//chi2    B3DB 驰

//chi2    DBE6 坻*

//chi2    DCAF 墀

//chi2    DCDD 茌

//chi2    F3F8 篪

//chi2    F5D8 踟

//chi3    B3DC 耻

//chi3    B3DD 齿

//chi3    B3DE 侈

//chi3    B3DF 尺*

//chi3    F1DD 褫

//chi3    F4F9 豉

//chi4    B3E0 赤

//chi4    B3E1 翅

//chi4    B3E2 斥

//chi4    B3E3 炽

//chi4    D9D1 傺

//chi4    DFB3 叱

//chi4    E0B4 啻

//chi4    E1DC 彳

//chi4    E2C1 饬

//chi4    EBB7 敕

//chi4    F1A1 瘛

//chong1  B3E4 充

//chong1  B3E5 冲*

//chong1  D3BF 涌*

//chong1  DCFB 茺

//chong1  E2E7 忡

//chong1  E3BF 憧

//chong1  F4A9 舂

//chong1  F4BE 艟

//chong2  B3E6 虫

//chong2  B3E7 崇

//chong2  D6D6 种*

//chong2  D6D8 重*

//chong3  B3E8 宠

//chong4  B3E5 冲*

//chong4  EFA5 铳

//chou1   B3E9 抽

//chou1   F1AC 瘳

//chou2   B3EA 酬

//chou2   B3EB 畴

//chou2   B3EC 踌

//chou2   B3ED 稠

//chou2   B3EE 愁

//chou2   B3EF 筹

//chou2   B3F0 仇*

//chou2   B3F1 绸

//chou2   D9B1 俦

//chou2   E0FC 帱*

//chou2   E3B0 惆

//chou2   F6C5 雠

//chou3   B3F2 瞅

//chou3   B3F3 丑

//chu1    B3F5 初

//chu1    B3F6 出

//chu1    E9CB 樗

//chu2    B3F7 橱

//chu2    B3F8 厨

//chu2    B3F9 躇

//chu2    B3FA 锄

//chu2    B3FB 雏

//chu2    B3FC 滁

//chu2    B3FD 除

//chu2    DBBB 刍

//chu2    F2DC 蜍

//chu2    F5E9 蹰

//chu3    B3FE 楚

//chu3    B4A1 础

//chu3    B4A2 储

//chu3    B4A6 处*

//chu3    E8C6 杵

//chu3    E8FA 楮

//chu3    F1D2 褚

//chu4    B4A3 矗

//chu4    B4A4 搐

//chu4    B4A5 触

//chu4    B4A6 处*

//chu4    D0F3 畜*

//chu4    D8A1 亍

//chu4    E2F0 怵

//chu4    E3C0 憷

//chu4    E7A9 绌

//chu4    F7ED 黜

//chuai1  B4A7 揣*

//chuai1  DEF5 搋

//chuai3  B4A7 揣*

//chuai4  B4A7 揣*

//chuai4  E0A8 啜*

//chuai4  E0DC 嘬*

//chuai4  EBFA 膪

//chuai4  F5DF 踹

//chuan1  B4A8 川

//chuan1  B4A9 穿

//chuan1  E7DD 巛

//chuan1  EBB0 氚

//chuan2  B4AA 椽

//chuan2  B4AB 传*

//chuan2  B4AC 船

//chuan2  E5D7 遄

//chuan2  F4AD 舡

//chuan3  B4AD 喘

//chuan3  E2B6 舛

//chuan4  B4AE 串

//chuan4  EECB 钏

//chuang1 B4AF 疮

//chuang1 B4B0 窗

//chuang1 B4B4 创*

//chuang2 B4B1 幢*

//chuang2 B4B2 床

//chuang3 B4B3 闯

//chuang4 B4B4 创*

//chuang4 E2EB 怆

//chui1   B4B5 吹

//chui1   B4B6 炊

//chui2   B4B7 捶

//chui2   B4B8 锤

//chui2   B4B9 垂

//chui2   D7B5 椎*

//chui2   DAEF 陲

//chui2   E9A2 棰

//chui2   E9B3 槌

//chun1   B4BA 春

//chun1   B4BB 椿

//chun1   F2ED 蝽

//chun2   B4BC 醇

//chun2   B4BD 唇

//chun2   B4BE 淳

//chun2   B4BF 纯

//chun2   DDBB 莼

//chun2   F0C8 鹑

//chun3   B4C0 蠢

//chuo1   B4C1 戳

//chuo1   F5D6 踔

//chuo4   B4C2 绰*

//chuo4   E0A8 啜*

//chuo4   EAA1 辍

//chuo4   F6BA 龊

//ci1     B2EE 差*

//ci1     B4C3 疵

//ci1     B4CC 刺*

//ci1     DFDA 呲

//ci2     B4C4 茨

//ci2     B4C5 磁

//ci2     B4C6 雌

//ci2     B4C7 辞

//ci2     B4C8 慈

//ci2     B4C9 瓷

//ci2     B4CA 词

//ci2     D7C8 兹*

//ci2     DCEB 茈*

//ci2     ECF4 祠

//ci2     F0CB 鹚

//ci2     F4D9 糍

//ci3     B4CB 此

//ci4     B4CC 刺*

//ci4     B4CD 赐

//ci4     B4CE 次

//ci4     CBC5 伺*

//cong1   B4CF 聪

//cong1   B4D0 葱

//cong1   B4D1 囱

//cong1   B4D2 匆

//cong1   DCCA 苁

//cong1   E6F5 骢

//cong1   E8AE 璁

//cong1   E8C8 枞*

//cong2   B4D3 从

//cong2   B4D4 丛

//cong2   E4C8 淙

//cong2   E7FD 琮

//cou4    B4D5 凑

//cou4    E9A8 楱

//cou4    EAA3 辏

//cou4    EBED 腠

//cu1     B4D6 粗

//cu2     E1DE 徂

//cu2     E9E3 殂

//cu4     B4D7 醋

//cu4     B4D8 簇

//cu4     B4D9 促

//cu4     D7E4 卒*

//cu4     DDFD 蔟

//cu4     E2A7 猝

//cu4     F5A1 酢*

//cu4     F5BE 蹙

//cu4     F5ED 蹴

//cuan1   B4DA 蹿

//cuan1   D9E0 汆

//cuan1   DFA5 撺

//cuan1   EFE9 镩

//cuan2   D4DC 攒*

//cuan4   B4DB 篡

//cuan4   B4DC 窜

//cuan4   ECE0 爨

//cui1    B4DD 摧

//cui1    B4DE 崔

//cui1    B4DF 催

//cui1    CBA5 衰

//cui1    E9C1 榱

//cui3    E8AD 璀

//cui4    B4E0 脆

//cui4    B4E1 瘁

//cui4    B4E2 粹

//cui4    B4E3 淬

//cui4    B4E4 翠

//cui4    DDCD 萃

//cui4    DFFD 啐

//cui4    E3B2 悴

//cui4    EBA5 毳

//cun1    B4E5 村

//cun1    F1E5 皴

//cun2    B4E6 存

//cun2    B6D7 蹲*

//cun3    E2E2 忖

//cun4    B4E7 寸

//cuo1    B4E8 磋

//cuo1    B4E9 撮*

//cuo1    B4EA 搓

//cuo1    F5E3 蹉

//cuo2    E1CF 嵯

//cuo2    EFF3 矬

//cuo2    F0EE 痤

//cuo2    F0FB 瘥*

//cuo2    F5BA 鹾

//cuo3    EBE2 脞

//cuo4    B4EB 措

//cuo4    B4EC 挫

//cuo4    B4ED 错

//cuo4    D8C8 厝

//cuo4    EFB1 锉

//da1     B4EE 搭

//da1     B4F0 答*

//da1     DEC7 耷

//da1     DFD5 哒

//da1     E0AA 嗒*

//da1     F1D7 褡

//da2     B4EF 达

//da2     B4F0 答*

//da2     B4F1 瘩*

//da2     B4F2 打*

//da2     E2F2 怛

//da2     E6A7 妲

//da2     EDB3 沓*

//da2     F3CE 笪

//da2     F7B0 靼

//da2     F7B2 鞑

//da3     B4F2 打*

//da4     B4F3 大*

//da5     B4F1 瘩*

//da5     F0E3 疸*

//dai1    B4F4 呆

//dai1    B4FD 待*

//dai1    DFBE 呔

//dai3    B4F5 歹

//dai3    B4F6 傣

//dai3    B4FE 逮*

//dai4    B4F3 大*

//dai4    B4F7 戴

//dai4    B4F8 带

//dai4    B4F9 殆

//dai4    B4FA 代

//dai4    B4FB 贷

//dai4    B4FC 袋

//dai4    B4FD 待*

//dai4    B4FE 逮*

//dai4    B5A1 怠

//dai4    DCA4 埭

//dai4    DFB0 甙

//dai4    E1B7 岱

//dai4    E5CA 迨

//dai4    E6E6 骀*

//dai4    E7AA 绐

//dai4    E7E9 玳

//dai4    F7EC 黛

//dan1    B5A2 耽

//dan1    B5A3 担*

//dan1    B5A4 丹

//dan1    B5A5 单*

//dan1    B5A6 郸

//dan1    D9D9 儋

//dan1    E9E9 殚

//dan1    EDF1 眈

//dan1    F0F7 瘅*

//dan1    F1F5 聃

//dan1    F3EC 箪

//dan3    B5A3 担*

//dan3    B5A7 掸*

//dan3    B5A8 胆

//dan3    EAE6 赕

//dan3    F0E3 疸*

//dan4    B5A3 担*

//dan4    B5A9 旦

//dan4    B5AA 氮

//dan4    B5AB 但

//dan4    B5AC 惮

//dan4    B5AD 淡

//dan4    B5AE 诞

//dan4    B5AF 弹*

//dan4    B5B0 蛋

//dan4    CAAF 石*

//dan4    DDCC 萏

//dan4    E0A2 啖

//dan4    E5A3 澹*

//dan4    F0F7 瘅*

//dang1   B5B1 当*

//dang1   EEF5 铛*

//dang1   F1C9 裆

//dang3   B5B2 挡*

//dang3   B5B3 党

//dang3   DAD4 谠

//dang4   B5B1 当*

//dang4   B5B2 挡*

//dang4   B5B4 荡

//dang4   B5B5 档

//dang4   DBCA 凼

//dang4   DDD0 菪

//dang4   E5B4 宕

//dang4   EDB8 砀

//dao1    B5B6 刀

//dao1    DFB6 叨*

//dao1    E2E1 忉

//dao1    EBAE 氘

//dao3    B5B7 捣

//dao3    B5B8 蹈

//dao3    B5B9 倒*

//dao3    B5BA 岛

//dao3    B5BB 祷

//dao3    B5BC 导

//dao4    B5B9 倒*

//dao4    B5BD 到

//dao4    B5BE 稻

//dao4    B5BF 悼

//dao4    B5C0 道

//dao4    B5C1 盗

//dao4    E0FC 帱*

//dao4    ECE2 焘*

//dao4    F4EE 纛

//de2     B5C2 德

//de2     B5C3 得*

//de2     EFBD 锝

//de5     B5C3 得*

//de5     B5C4 的*

//de5     B5D7 底*

//de5     B5D8 地*

//dei3    B5C3 得*

//deng1   B5C5 蹬*

//deng1   B5C6 灯

//deng1   B5C7 登

//deng1   E0E2 噔

//deng1   F4A3 簦

//deng3   B5C8 等

//deng3   EAAD 戥

//deng4   B3CE 澄*

//deng4   B5C5 蹬*

//deng4   B5C9 瞪

//deng4   B5CA 凳

//deng4   B5CB 邓

//deng4   E1D8 嶝

//deng4   EDE3 磴

//deng4   EFEB 镫

//di1     B5CC 堤

//di1     B5CD 低

//di1     B5CE 滴

//di1     CCE1 提*

//di1     D8B5 氐*

//di1     E0D6 嘀*

//di1     EFE1 镝*

//di1     F4C6 羝

//di2     B5C4 的*

//di2     B5CF 迪

//di2     B5D0 敌

//di2     B5D1 笛

//di2     B5D2 狄

//di2     B5D3 涤

//di2     B5D4 翟*

//di2     B5D5 嫡

//di2     D9E1 籴

//di2     DDB6 荻

//di2     E0D6 嘀*

//di2     EAEB 觌

//di2     EFE1 镝*

//di3     B5D6 抵

//di3     B5D7 底*

//di3     D8B5 氐*

//di3     DAAE 诋

//di3     DBA1 邸

//di3     DBE6 坻*

//di3     E8DC 柢

//di3     EDC6 砥

//di3     F7BE 骶

//di4     B5C4 的*

//di4     B5D8 地*

//di4     B5D9 蒂

//di4     B5DA 第

//di4     B5DB 帝

//di4     B5DC 弟

//di4     B5DD 递

//di4     B5DE 缔

//di4     DAD0 谛

//di4     E6B7 娣

//di4     E9A6 棣

//di4     EDDA 碲

//di4     EDFB 睇

//dia3    E0C7 嗲

//dian1   B5DF 颠

//dian1   B5E0 掂

//dian1   B5E1 滇

//dian1   E1DB 巅

//dian1   F1B2 癫

//dian3   B5E2 碘

//dian3   B5E3 点

//dian3   B5E4 典

//dian3   F5DA 踮

//dian4   B5E5 靛

//dian4   B5E6 垫

//dian4   B5E7 电

//dian4   B5E8 佃*

//dian4   B5E9 甸

//dian4   B5EA 店

//dian4   B5EB 惦

//dian4   B5EC 奠

//dian4   B5ED 淀

//dian4   B5EE 殿

//dian4   DAE7 阽*

//dian4   DBE3 坫

//dian4   E7E8 玷

//dian4   EEE4 钿*

//dian4   F1B0 癜

//dian4   F4A1 簟

//diao1   B5EF 碉

//diao1   B5F0 叼

//diao1   B5F1 雕

//diao1   B5F2 凋

//diao1   B5F3 刁

//diao1   F5F5 貂

//diao1   F6F4 鲷

//diao3   C4F1 鸟*

//diao4   B5F4 掉

//diao4   B5F5 吊

//diao4   B5F6 钓

//diao4   B5F7 调*

//diao4   EEF6 铞

//diao4   EFA2 铫*

//die1    B5F8 跌

//die1    B5F9 爹

//die2    B5FA 碟

//die2    B5FB 蝶

//die2    B5FC 迭

//die2    B5FD 谍

//die2    B5FE 叠

//die2    DBEC 垤

//die2    DCA6 堞

//die2    DEE9 揲*

//die2    E0A9 喋

//die2    EBBA 牒

//die2    F0AC 瓞

//die2    F1F3 耋

//die2    F5DE 蹀

//die2    F6F8 鲽

//ding1   B6A1 丁*

//ding1   B6A2 盯

//ding1   B6A3 叮

//ding1   B6A4 钉*

//ding1   D8EA 仃

//ding1   E7E0 玎

//ding1   EEAE 町*

//ding1   F0DB 疔

//ding1   F1F4 耵

//ding1   F4FA 酊*

//ding3   B6A5 顶

//ding3   B6A6 鼎

//ding3   F4FA 酊*

//ding4   B6A4 钉*

//ding4   B6A7 锭

//ding4   B6A8 定

//ding4   B6A9 订

//ding4   E0A4 啶

//ding4   EBEB 腚

//ding4   EDD6 碇

//ding4   EEFA 铤*

//ding4   EEFA 铤*

//diu1    B6AA 丢

//diu1    EEFB 铥

//dong1   B6AB 东

//dong1   B6AC 冬

//dong1   DFCB 咚

//dong1   E1B4 岽

//dong1   EBB1 氡

//dong1   F0B4 鸫

//dong3   B6AD 董

//dong3   B6AE 懂

//dong4   B6AF 动

//dong4   B6B0 栋

//dong4   B6B1 侗*

//dong4   B6B2 恫

//dong4   B6B3 冻

//dong4   B6B4 洞

//dong4   DBED 垌*

//dong4   E1BC 峒*

//dong4   EBCB 胨

//dong4   EBD8 胴

//dong4   EDCF 硐

//dou1    B6B5 兜

//dou1    B6BC 都*

//dou1    DDFA 蔸

//dou1    F3FB 篼

//dou3    B6B6 抖

//dou3    B6B7 斗*

//dou3    B6B8 陡

//dou3    F2BD 蚪

//dou4    B6B7 斗*

//dou4    B6B9 豆

//dou4    B6BA 逗

//dou4    B6BB 痘

//dou4    B6C1 读*

//dou4    F1BC 窦

//du1     B6BC 都*

//du1     B6BD 督

//du1     E0BD 嘟

//du2     B6BE 毒

//du2     B6BF 犊

//du2     B6C0 独

//du2     B6C1 读*

//du2     B6D9 顿*

//du2     E4C2 渎

//du2     E8FC 椟

//du2     EBB9 牍

//du2     F7C7 髑

//du2     F7F2 黩

//du3     B6C2 堵

//du3     B6C3 睹

//du3     B6C4 赌

//du3     B6C7 肚*

//du3     F3C6 笃

//du4     B6C5 杜

//du4     B6C6 镀

//du4     B6C7 肚*

//du4     B6C8 度

//du4     B6C9 渡

//du4     B6CA 妒

//du4     DCB6 芏

//du4     F3BC 蠹

//duan1   B6CB 端

//duan3   B6CC 短

//duan4   B6CD 锻

//duan4   B6CE 段

//duan4   B6CF 断

//duan4   B6D0 缎

//duan4   E9B2 椴

//duan4   ECD1 煅

//duan4   F3FD 簖

//dui1    B6D1 堆

//dui4    B6D2 兑

//dui4    B6D3 队

//dui4    B6D4 对

//dui4    B6D8 敦*

//dui4    EDA1 怼

//dui4    EDAD 憝

//dui4    EDD4 碓

//dui4    EFE6 镦

//dun1    B6D5 墩

//dun1    B6D6 吨

//dun1    B6D7 蹲*

//dun1    B6D8 敦*

//dun1    EDE2 礅

//dun3    EDEF 盹

//dun3    F5BB 趸

//dun4    B6D9 顿*

//dun4    B6DA 囤*

//dun4    B6DB 钝

//dun4    B6DC 盾

//dun4    B6DD 遁

//dun4    E3E7 沌

//dun4    ECC0 炖

//dun4    EDBB 砘

//duo1    B6DE 掇

//duo1    B6DF 哆

//duo1    B6E0 多

//duo1    DFCD 咄

//duo1    F1D6 裰

//duo2    B6E1 夺

//duo2    EEEC 铎

//duo2    F5E2 踱

//duo3    B6E2 垛*

//duo3    B6E3 躲

//duo3    B6E4 朵

//duo3    DFE1 哚

//duo3    E7B6 缍

//duo4    B6E2 垛*

//duo4    B6E5 跺

//duo4    B6E6 舵

//duo4    B6E7 剁

//duo4    B6E8 惰

//duo4    B6E9 堕

//duo4    CDD4 驮*

//duo4    E3F5 沲

//duo4    E8DE 柁*

//e1      B0A2 阿*

//e1      E5ED 屙

//e1      E6B9 婀

//e2      B6EA 蛾

//e2      B6EB 峨

//e2      B6EC 鹅

//e2      B6ED 俄

//e2      B6EE 额

//e2      B6EF 讹

//e2      B6F0 娥

//e2      C5B6 哦*

//e2      DDAD 莪

//e2      EFB0 锇

//e3      B6F1 恶*

//e4      B6F1 恶*

//e4      B6F2 厄

//e4      B6F3 扼

//e4      B6F4 遏

//e4      B6F5 鄂

//e4      B6F6 饿

//e4      D8AC 噩

//e4      DACC 谔

//e4      DBD1 垩

//e4      DCC3 苊

//e4      DDE0 萼

//e4      DFC0 呃

//e4      E3B5 愕

//e4      E3D5 阏*

//e4      E9EE 轭

//e4      EBF1 腭

//e4      EFC9 锷

//e4      F0CA 鹗

//e4      F2A6 颚

//e4      F6F9 鳄

//�1      DAC0 诶*

//�2      DAC0 诶*

//�3      DAC0 诶*

//�4      DAC0 诶*

//ei2     DAC0 诶*

//ei3     DAC0 诶*

//ei4     DAC0 诶*

//en1     B6F7 恩

//en1     DDEC 蒽

//en4     DEF4 摁

//er2     B6F8 而

//er2     B6F9 儿

//er2     F0B9 鸸

//er2     F6DC 鲕

//er3     B6FA 耳

//er3     B6FB 尔

//er3     B6FC 饵

//er3     B6FD 洱

//er3     E5C7 迩

//er3     E7ED 珥

//er3     EEEF 铒

//er4     B6FE 二

//er4     B7A1 贰

//er4     D9A6 佴*

//fa1     B7A2 发*

//fa2     B7A3 罚

//fa2     B7A4 筏

//fa2     B7A5 伐

//fa2     B7A6 乏

//fa2     B7A7 阀

//fa2     DBD2 垡

//fa3     B7A8 法

//fa3     EDC0 砝

//fa4     B7A2 发*

//fa4     B7A9 珐

//fan1    B7AA 藩

//fan1    B7AB 帆

//fan1    B7AC 番*

//fan1    B7AD 翻

//fan1    DEAC 蕃*

//fan1    E1A6 幡

//fan2    B7AE 樊

//fan2    B7AF 矾

//fan2    B7B0 钒

//fan2    B7B1 繁*

//fan2    B7B2 凡

//fan2    B7B3 烦

//fan2    DEAC 蕃*

//fan2    DEC0 蘩

//fan2    ECDC 燔

//fan2    F5EC 蹯

//fan3    B7B4 反

//fan3    B7B5 返

//fan4    B7B6 范

//fan4    B7B7 贩

//fan4    B7B8 犯

//fan4    B7B9 饭

//fan4    B7BA 泛

//fan4    E8F3 梵

//fan4    EEB2 畈

//fang1   B7BB 坊*

//fang1   B7BC 芳

//fang1   B7BD 方

//fang1   DAFA 邡

//fang1   E8CA 枋

//fang1   EED5 钫

//fang2   B7BB 坊*

//fang2   B7BE 肪

//fang2   B7BF 房

//fang2   B7C0 防

//fang2   B7C1 妨

//fang2   F6D0 鲂

//fang3   B7C2 仿

//fang3   B7C3 访

//fang3   B7C4 纺

//fang3   E1DD 彷*

//fang3   F4B3 舫

//fang4   B7C5 放

//fei1    B7C6 菲*

//fei1    B7C7 非

//fei1    B7C8 啡

//fei1    B7C9 飞

//fei1    E5FA 妃

//fei1    E7B3 绯

//fei1    ECE9 扉

//fei1    F2E3 蜚*

//fei1    F6AD 霏

//fei1    F6EE 鲱

//fei2    B7CA 肥

//fei2    E4C7 淝

//fei2    EBE8 腓

//fei3    B7C6 菲*

//fei3    B7CB 匪

//fei3    B7CC 诽

//fei3    E3AD 悱

//fei3    E9BC 榧

//fei3    ECB3 斐

//fei3    F2E3 蜚*

//fei3    F3F5 篚

//fei3    F4E4 翡

//fei4    B7CD 吠

//fei4    B7CE 肺

//fei4    B7CF 废

//fei4    B7D0 沸

//fei4    B7D1 费

//fei4    DCC0 芾*

//fei4    E1F4 狒

//fei4    E2F6 怫*

//fei4    EFD0 镄

//fei4    F0F2 痱

//fen1    B7D2 芬

//fen1    B7D3 酚

//fen1    B7D4 吩

//fen1    B7D5 氛

//fen1    B7D6 分*

//fen1    B7D7 纷

//fen1    E7E3 玢*

//fen2    B7D8 坟

//fen2    B7D9 焚

//fen2    B7DA 汾

//fen2    E8FB 棼

//fen2    F7F7 鼢

//fen3    B7DB 粉

//fen4    B7D6 分*

//fen4    B7DC 奋

//fen4    B7DD 份

//fen4    B7DE 忿

//fen4    B7DF 愤

//fen4    B7E0 粪

//fen4    D9C7 偾

//fen4    E5AF 瀵

//fen4    F6F7 鲼

//feng1   B7E1 丰

//feng1   B7E2 封

//feng1   B7E3 枫

//feng1   B7E4 蜂

//feng1   B7E5 峰

//feng1   B7E6 锋

//feng1   B7E7 风

//feng1   B7E8 疯

//feng1   B7E9 烽

//feng1   DBBA 酆

//feng1   DDD7 葑*

//feng1   E3E3 沣

//feng1   EDBF 砜

//feng2   B7EA 逢

//feng2   B7EB 冯

//feng2   B7EC 缝*

//feng3   B7ED 讽

//feng3   DFF4 唪

//feng4   B7EC 缝*

//feng4   B7EE 奉

//feng4   B7EF 凤

//feng4   D9BA 俸

//feng4   DDD7 葑*

//fo2     B7F0 佛*

//fou3    B7F1 否*

//fou3    F3BE 缶

//fu1     B7F2 夫*

//fu1     B7F3 敷

//fu1     B7F4 肤

//fu1     B7F5 孵

//fu1     DFBB 呋

//fu1     EFFB 稃

//fu1     F4EF 麸

//fu1     F5C3 趺

//fu1     F5C6 跗

//fu2     B7F0 佛*

//fu2     B7F2 夫*

//fu2     B7F6 扶

//fu2     B7F7 拂

//fu2     B7F8 辐

//fu2     B7F9 幅

//fu2     B7FA 氟

//fu2     B7FB 符

//fu2     B7FC 伏

//fu2     B7FD 俘

//fu2     B7FE 服*

//fu2     B8A1 浮

//fu2     B8A2 涪

//fu2     B8A3 福

//fu2     B8A4 袱

//fu2     B8A5 弗

//fu2     D9EB 匐

//fu2     D9EC 凫

//fu2     DBAE 郛

//fu2     DCBD 芙

//fu2     DCC0 芾*

//fu2     DCDE 苻

//fu2     DCF2 茯

//fu2     DDB3 莩*

//fu2     DDCA 菔

//fu2     E1A5 幞

//fu2     E2F6 怫*

//fu2     E5F5 艴

//fu2     E6DA 孚

//fu2     E7A6 绂

//fu2     E7A8 绋

//fu2     E8F5 桴

//fu2     ECF0 祓

//fu2     EDC9 砩

//fu2     EDEA 黻

//fu2     EEB7 罘

//fu2     F2B6 蚨

//fu2     F2DD 蜉

//fu2     F2F0 蝠

//fu3     B8A6 甫

//fu3     B8A7 抚

//fu3     B8A8 辅

//fu3     B8A9 俯

//fu3     B8AA 釜

//fu3     B8AB 斧

//fu3     B8AC 脯*

//fu3     B8AD 腑

//fu3     B8AE 府

//fu3     B8AF 腐

//fu3     B8B8 父*

//fu3     DED4 拊

//fu3     E4E6 滏

//fu3     EDEB 黼

//fu4     B7FE 服*

//fu4     B8B0 赴

//fu4     B8B1 副

//fu4     B8B2 覆

//fu4     B8B3 赋

//fu4     B8B4 复

//fu4     B8B5 傅

//fu4     B8B6 付

//fu4     B8B7 阜

//fu4     B8B8 父*

//fu4     B8B9 腹

//fu4     B8BA 负

//fu4     B8BB 富

//fu4     B8BC 讣

//fu4     B8BD 附

//fu4     B8BE 妇

//fu4     B8BF 缚

//fu4     B8C0 咐

//fu4     E6E2 驸

//fu4     EAE7 赙

//fu4     F0A5 馥

//fu4     F2F3 蝮

//fu4     F6D6 鲋

//fu4     F6FB 鳆

//ga1     B8C2 嘎*

//ga1     B8EC 胳

//ga1     BCD0 夹*

//ga1     BFA7 咖*

//ga1     D9A4 伽

//ga1     EAB8 旮

//ga2     B8C1 噶

//ga2     B8C2 嘎*

//ga2     D4FE 轧*

//ga2     E6D9 尜

//ga2     EEC5 钆

//ga3     B8C2 嘎*

//ga3     E6D8 尕

//ga4     DECE 尬

//gai1    B8C3 该

//gai1    DAEB 陔

//gai1    DBF2 垓

//gai1    EAE0 赅

//gai3    B8C4 改

//gai4    B8C5 概

//gai4    B8C6 钙

//gai4    B8C7 盖*

//gai4    B8C8 溉

//gai4    BDE6 芥*

//gai4    D8A4 丐

//gai4    EAAE 戤

//gan1    B8C9 干*

//gan1    B8CA 甘

//gan1    B8CB 杆*

//gan1    B8CC 柑

//gan1    B8CD 竿

//gan1    B8CE 肝

//gan1    C7AC 乾*

//gan1    DBE1 坩

//gan1    DCD5 苷

//gan1    DECF 尴

//gan1    E3EF 泔

//gan1    EDB7 矸

//gan1    F0E1 疳

//gan1    F4FB 酐

//gan3    B8CB 杆*

//gan3    B8CF 赶

//gan3    B8D0 感

//gan3    B8D1 秆

//gan3    B8D2 敢

//gan3    DFA6 擀

//gan3    E4F7 澉

//gan3    E9CF 橄

//gan4    B8C9 干*

//gan4    B8D3 赣

//gan4    E4C6 淦

//gan4    E7A4 绀

//gan4    EABA 旰

//gang1   B8D4 冈

//gang1   B8D5 刚

//gang1   B8D6 钢*

//gang1   B8D7 缸

//gang1   B8D8 肛

//gang1   B8D9 纲

//gang1   B8DA 岗*

//gang1   B8DC 杠*

//gang1   BFB8 扛*

//gang1   EEB8 罡

//gang3   B8DA 岗*

//gang3   B8DB 港

//gang4   B8D6 钢*

//gang4   B8DC 杠*

//gang4   EDB0 戆*

//gang4   F3E0 筻

//gao1    B8DD 篙

//gao1    B8DE 皋

//gao1    B8DF 高

//gao1    B8E0 膏*

//gao1    B8E1 羔

//gao1    B8E2 糕

//gao1    D8BA 睾

//gao1    E9C0 槔

//gao3    B8E3 搞

//gao3    B8E4 镐*

//gao3    B8E5 稿

//gao3    DEBB 藁

//gao3    E7C9 缟

//gao3    E9C2 槁

//gao3    EABD 杲

//gao4    B8E0 膏*

//gao4    B8E6 告

//gao4    DABE 诰

//gao4    DBAC 郜

//gao4    EFAF 锆

//ge1     B8E7 哥

//ge1     B8E8 歌

//ge1     B8E9 搁*

//ge1     B8EA 戈

//ge1     B8EB 鸽

//ge1     B8ED 疙

//ge1     B8EE 割

//ge1     B8F1 格*

//ge1     BFA9 咯*

//ge1     D2D9 屹*

//ge1     D8EE 仡

//ge1     DBD9 圪

//ge1     E6FC 纥*

//ge1     F1CB 袼

//ge2     B8E9 搁*

//ge2     B8EF 革*

//ge2     B8F0 葛*

//ge2     B8F1 格*

//ge2     B8F2 蛤*

//ge2     B8F3 阁

//ge2     B8F4 隔

//ge2     D8AA 鬲*

//ge2     DCAA 塥

//ge2     E0C3 嗝

//ge2     EBA1 搿

//ge2     EBF5 膈

//ge2     EFD3 镉

//ge2     F2A2 颌*

//ge2     F7C0 骼

//ge3     B8C7 盖*

//ge3     B8F0 葛*

//ge3     B8F6 个*

//ge3     B8F7 各*

//ge3     BACF 合*

//ge3     DBC1 哿

//ge3     F4B4 舸

//ge4     B8F5 铬

//ge4     B8F6 个*

//ge4     B8F7 各*

//ge4     EDD1 硌*

//ge4     F2B4 虼

//gei3    B8F8 给*

//gen1    B8F9 根

//gen1    B8FA 跟

//gen2    DFE7 哏

//gen3    F4DE 艮*

//gen4    D8A8 亘

//gen4    DDA2 茛

//gen4    F4DE 艮*

//geng1   B8FB 耕

//geng1   B8FC 更*

//geng1   B8FD 庚

//geng1   B8FE 羹

//geng1   E2D9 赓

//geng3   B9A1 埂

//geng3   B9A2 耿

//geng3   B9A3 梗

//geng3   BEB1 颈*

//geng3   DFEC 哽

//geng3   E7AE 绠

//geng3   F6E1 鲠

//geng4   B8FC 更*

//gong1   B9A4 工

//gong1   B9A5 攻

//gong1   B9A6 功

//gong1   B9A7 恭

//gong1   B9A8 龚

//gong1   B9A9 供*

//gong1   B9AA 躬

//gong1   B9AB 公

//gong1   B9AC 宫

//gong1   B9AD 弓

//gong1   BAEC 红*

//gong1   EBC5 肱

//gong1   F2BC 蚣

//gong1   F6A1 觥

//gong3   B9AE 巩

//gong3   B9AF 汞

//gong3   B9B0 拱

//gong3   E7EE 珙

//gong4   B9A9 供*

//gong4   B9B1 贡

//gong4   B9B2 共

//gou1    B9B3 钩

//gou1    B9B4 勾*

//gou1    B9B5 沟

//gou1    BEE4 句*

//gou1    D8FE 佝

//gou1    E7C3 缑

//gou1    E8DB 枸*

//gou1    F3F4 篝

//gou1    F7B8 鞲

//gou3    B9B6 苟

//gou3    B9B7 狗

//gou3    E1B8 岣

//gou3    E8DB 枸*

//gou3    F3D1 笱

//gou4    B9B4 勾*

//gou4    B9B8 垢

//gou4    B9B9 构

//gou4    B9BA 购

//gou4    B9BB 够

//gou4    DAB8 诟

//gou4    E5DC 遘

//gou4    E6C5 媾

//gou4    EAED 觏

//gou4    ECB0 彀

//gu1     B9BC 辜

//gu1     B9BD 菇

//gu1     B9BE 咕

//gu1     B9BF 箍

//gu1     B9C0 估*

//gu1     B9C1 沽

//gu1     B9C2 孤

//gu1     B9C3 姑

//gu1     B9C7 骨*

//gu1     DDD4 菰

//gu1     DFC9 呱*

//gu1     E9EF 轱

//gu1     ECB1 毂*

//gu1     F0B3 鸪

//gu1     F2C1 蛄

//gu1     F4FE 酤

//gu1     F5FD 觚

//gu3     B9C4 鼓*

//gu3     B9C5 古

//gu3     B9C6 蛊

//gu3     B9C7 骨*

//gu3     B9C8 谷*

//gu3     B9C9 股

//gu3     BCD6 贾*

//gu3     D8C5 嘏*

//gu3     DAAC 诂

//gu3     E3E9 汩

//gu3     EAF4 牯

//gu3     EBFB 臌

//gu3     ECB1 毂*

//gu3     EEAD 瞽

//gu3     EEB9 罟

//gu3     EEDC 钴

//gu3     F0C0 鹄

//gu3     F7BD 鹘*

//gu4     B9C0 估*

//gu4     B9CA 故

//gu4     B9CB 顾

//gu4     B9CC 固

//gu4     B9CD 雇

//gu4     E1C4 崮

//gu4     E8F4 梏

//gu4     EAF6 牿

//gu4     EFC0 锢

//gu4     F0F3 痼

//gu4     F6F1 鲴

//gua1    B9CE 刮

//gua1    B9CF 瓜

//gua1    C0A8 括*

//gua1    DFC9 呱*

//gua1    E8E9 栝

//gua1    EBD2 胍

//gua1    F0BB 鸹

//gua3    B9D0 剐

//gua3    B9D1 寡

//gua3    DFC9 呱*

//gua4    B9D2 挂

//gua4    B9D3 褂

//gua4    D8D4 卦

//gua4    DAB4 诖

//guai1   B9D4 乖

//guai1   DEE2 掴*

//guai3   B9D5 拐

//guai4   B9D6 怪

//guan1   B9D7 棺

//guan1   B9D8 关

//guan1   B9D9 官

//guan1   B9DA 冠*

//guan1   B9DB 观*

//guan1   C2DA 纶*

//guan1   D9C4 倌

//guan1   DDB8 莞*

//guan1   F1E6 矜*

//guan1   F7A4 鳏

//guan3   B9DC 管

//guan3   B9DD 馆

//guan4   B9DA 冠*

//guan4   B9DB 观*

//guan4   B9DE 罐

//guan4   B9DF 惯

//guan4   B9E0 灌

//guan4   B9E1 贯

//guan4   DEE8 掼

//guan4   E4CA 涫

//guan4   EEC2 盥

//guan4   F0D9 鹳

//guang1  B9E2 光

//guang1  DFDB 咣

//guang1  E8E6 桄*

//guang1  EBD7 胱

//guang3  B9E3 广*

//guang3  E1EE 犷

//guang4  B9E4 逛

//guang4  E8E6 桄*

//gui1    B9E5 瑰

//gui1    B9E6 规

//gui1    B9E7 圭

//gui1    B9E8 硅

//gui1    B9E9 归

//gui1    B9EA 龟*

//gui1    B9EB 闺

//gui1    BFFE 傀*

//gui1    E6A3 妫

//gui1    F0A7 皈

//gui1    F6D9 鲑

//gui3    B9EC 轨

//gui3    B9ED 鬼

//gui3    B9EE 诡

//gui3    B9EF 癸

//gui3    D8D0 匦

//gui3    E2D1 庋

//gui3    E5B3 宄

//gui3    EAD0 晷

//gui3    F3FE 簋

//gui4    B9F0 桂

//gui4    B9F1 柜*

//gui4    B9F2 跪

//gui4    B9F3 贵

//gui4    B9F4 刽

//gui4    C8B2 炔*

//gui4    D8DB 刿

//gui4    E8ED 桧*

//gui4    EAC1 炅*

//gui4    F7AC 鳜

//gun3    B9F5 辊

//gun3    B9F6 滚

//gun3    D9F2 衮

//gun3    E7B5 绲

//gun3    EDDE 磙

//gun3    F6E7 鲧

//gun4    B9F7 棍

//guo1    B9F8 锅

//guo1    B9F9 郭

//guo1    B9FD 过*

//guo1    CED0 涡*

//guo1    DBF6 埚

//guo1    DFC3 呙

//guo1    E1C6 崞

//guo1    F1F8 聒

//guo1    F2E5 蝈

//guo2    B9FA 国

//guo2    D9E5 馘

//guo2    DEE2 掴*

//guo2    E0FE 帼

//guo2    EBBD 虢

//guo3    B9FB 果

//guo3    B9FC 裹

//guo3    E2A3 猓

//guo3    E9A4 椁

//guo3    F2E4 蜾

//guo4    B9FD 过*

//ha1     B9FE 哈*

//ha1     EEFE 铪

//ha2     B8F2 蛤*

//ha2     CFBA 虾*

//ha3     B9FE 哈*

//ha4     B9FE 哈*

//hai1    BFC8 咳*

//hai1    E0CB 嗨*

//hai2    BAA1 骸

//hai2    BAA2 孩

//hai2    BBB9 还*

//hai3    BAA3 海

//hai3    EBDC 胲

//hai3    F5B0 醢

//hai4    BAA4 氦

//hai4    BAA5 亥

//hai4    BAA6 害

//hai4    BAA7 骇

//han1    BAA8 酣

//han1    BAA9 憨

//han1    E1ED 犴*

//han1    F1FC 顸

//han1    F2C0 蚶

//han1    F7FD 鼾

//han2    BAAA 邯

//han2    BAAB 韩

//han2    BAAC 含

//han2    BAAD 涵

//han2    BAAE 寒

//han2    BAAF 函

//han2    BAB9 汗*

//han2    DAF5 邗

//han2    EACF 晗

//han2    ECCA 焓

//han3    BAB0 喊

//han3    BAB1 罕

//han3    E3DB 阚*

//han4    BAB2 翰

//han4    BAB3 撼

//han4    BAB4 捍

//han4    BAB5 旱

//han4    BAB6 憾

//han4    BAB7 悍

//han4    BAB8 焊

//han4    BAB9 汗*

//han4    BABA 汉

//han4    DDD5 菡

//han4    DEFE 撖

//han4    E5AB 瀚

//han4    F2A5 颔

//hang1   BABB 夯*

//hang2   BABC 杭

//hang2   BABD 航

//hang2   BFD4 吭*

//hang2   D0D0 行*

//hang2   E7AC 绗

//hang2   F1FE 颃

//hang4   CFEF 巷*

//hang4   E3EC 沆

//hao1    DDEF 蒿

//hao1    DEB6 薅

//hao1    E0E3 嚆

//hao2    BABE 壕

//hao2    BABF 嚎

//hao2    BAC0 豪

//hao2    BAC1 毫

//hao2    BAC5 号*

//hao2    BAD1 貉*

//hao2    E0C6 嗥

//hao2    E5A9 濠

//hao2    F2BA 蚝

//hao3    BAC2 郝

//hao3    BAC3 好*

//hao4    B8E4 镐*

//hao4    BAC3 好*

//hao4    BAC4 耗

//hao4    BAC5 号*

//hao4    BAC6 浩

//hao4    E5B0 灏

//hao4    EABB 昊

//hao4    F0A9 皓

//hao4    F2AB 颢

//he1     BAC7 呵*

//he1     BAC8 喝*

//he1     DAAD 诃

//he1     E0C0 嗬

//he2     BAC9 荷*

//he2     BACA 菏

//he2     BACB 核*

//he2     BACC 禾

//he2     BACD 和*

//he2     BACE 何

//he2     BACF 合*

//he2     BAD0 盒

//he2     BAD1 貉*

//he2     BAD2 阂

//he2     BAD3 河

//he2     BAD4 涸

//he2     DBC0 劾

//he2     E3D8 阖

//he2     E6FC 纥*

//he2     EAC2 曷

//he2     EEC1 盍

//he2     F2A2 颌*

//he2     F2C2 蚵

//he2     F4E7 翮

//he4     BAC8 喝*

//he4     BAC9 荷*

//he4     BACD 和*

//he4     BAD5 赫

//he4     BAD6 褐

//he4     BAD7 鹤

//he4     BAD8 贺

//he4     CFC5 吓*

//he4     DBD6 壑

//hei1    BAD9 嘿*

//hei1    BADA 黑

//hei1    E0CB 嗨*

//hen2    BADB 痕

//hen3    BADC 很

//hen3    BADD 狠

//hen4    BADE 恨

//heng1   BADF 哼*

//heng1   BAE0 亨

//heng2   BAE1 横*

//heng2   BAE2 衡

//heng2   BAE3 恒

//heng2   DEBF 蘅

//heng2   E7F1 珩

//heng2   E8EC 桁

//heng4   BAE1 横*

//hng5    BADF 哼*

//hong1   BAE4 轰

//hong1   BAE5 哄*

//hong1   BAE6 烘

//hong1   D9EA 訇

//hong1   DEB0 薨

//hong2   BAE7 虹*

//hong2   BAE8 鸿

//hong2   BAE9 洪

//hong2   BAEA 宏

//hong2   BAEB 弘

//hong2   BAEC 红*

//hong2   D9E4 黉

//hong2   DDA6 荭

//hong2   DEAE 蕻*

//hong2   E3C8 闳

//hong2   E3FC 泓

//hong3   BAE5 哄*

//hong4   BAE5 哄*

//hong4   DAA7 讧

//hong4   DEAE 蕻*

//hou2    BAED 喉

//hou2    BAEE 侯*

//hou2    BAEF 猴

//hou2    F0FA 瘊

//hou2    F3F3 篌

//hou2    F4D7 糇

//hou2    F7BF 骺

//hou3    BAF0 吼

//hou4    BAEE 侯*

//hou4    BAF1 厚

//hou4    BAF2 候

//hou4    BAF3 后

//hou4    DCA9 堠

//hou4    E1E1 後

//hou4    E5CB 逅

//hou4    F6D7 鲎

//hu1     BAF4 呼

//hu1     BAF5 乎

//hu1     BAF6 忽

//hu1     BAFD 糊*

//hu1     CFB7 戏*

//hu1     DFFC 唿

//hu1     E3B1 惚

//hu1     E4EF 滹

//hu1     E9F5 轷

//hu1     ECC3 烀

//hu2     B9C4 鼓*

//hu2     BACB 核*

//hu2     BACD 和*

//hu2     BAF7 瑚

//hu2     BAF8 壶

//hu2     BAF9 葫

//hu2     BAFA 胡

//hu2     BAFB 蝴

//hu2     BAFC 狐

//hu2     BAFD 糊*

//hu2     BAFE 湖

//hu2     BBA1 弧

//hu2     E0F1 囫

//hu2     E2A9 猢

//hu2     E9CE 槲

//hu2     ECB2 觳

//hu2     ECCE 煳

//hu2     F0C9 鹕

//hu2     F5AD 醐

//hu2     F5FA 斛

//hu2     F7BD 鹘*

//hu3     BBA2 虎

//hu3     BBA3 唬*

//hu3     E4B0 浒*

//hu3     E7FA 琥

//hu4     BAFD 糊*

//hu4     BBA4 护

//hu4     BBA5 互

//hu4     BBA6 沪

//hu4     BBA7 户

//hu4     D9FC 冱

//hu4     E1B2 岵

//hu4     E2EF 怙

//hu4     ECE6 戽

//hu4     ECE8 扈

//hu4     ECEF 祜

//hu4     F0AD 瓠

//hu4     F0D7 鹱

//hu4     F3CB 笏

//hua1    BBA8 花

//hua1    BBA9 哗*

//hua1    BBAA 华*

//hua1    BBAF 化*

//hua1    EDB9 砉

//hua2    BBA9 哗*

//hua2    BBAA 华*

//hua2    BBAB 猾

//hua2    BBAC 滑

//hua2    BBAE 划*

//hua2    E6E8 骅

//hua2    EEFC 铧

//hua4    BBAA 华*

//hua4    BBAD 画

//hua4    BBAE 划*

//hua4    BBAF 化*

//hua4    BBB0 话

//hua4    E8EB 桦

//huai2   BBB1 槐

//huai2   BBB2 徊

//huai2   BBB3 怀

//huai2   BBB4 淮

//huai2   F5D7 踝

//huai4   BBB5 坏*

//huai5   BBAE 划*

//huan1   BBB6 欢

//huan1   E2B5 獾

//huan2   BBB7 环

//huan2   BBB8 桓

//huan2   BBB9 还*

//huan2   DBA8 郇*

//huan2   DDC8 萑

//huan2   E0F7 圜*

//huan2   E4A1 洹

//huan2   E5BE 寰

//huan2   E7D9 缳

//huan2   EFCC 锾

//huan2   F7DF 鬟

//huan3   BBBA 缓

//huan4   BBBB 换

//huan4   BBBC 患

//huan4   BBBD 唤

//huan4   BBBE 痪

//huan4   BBBF 豢

//huan4   BBC0 焕

//huan4   BBC1 涣

//huan4   BBC2 宦

//huan4   BBC3 幻

//huan4   DBBC 奂

//huan4   DFA7 擐

//huan4   E4BD 浣

//huan4   E4F1 漶

//huan4   E5D5 逭

//huan4   F6E9 鲩

//huang1  BBC4 荒

//huang1  BBC5 慌

//huang1  EBC1 肓

//huang2  BBC6 黄

//huang2  BBC7 磺

//huang2  BBC8 蝗

//huang2  BBC9 簧

//huang2  BBCA 皇

//huang2  BBCB 凰

//huang2  BBCC 惶

//huang2  BBCD 煌

//huang2  DAF2 隍

//huang2  E1E5 徨

//huang2  E4D2 湟

//huang2  E4EA 潢

//huang2  E5D8 遑

//huang2  E8AB 璜

//huang2  F1A5 癀

//huang2  F3A8 蟥

//huang2  F3F2 篁

//huang2  F6FC 鳇

//huang3  BBCE 晃*

//huang3  BBCF 幌

//huang3  BBD0 恍

//huang3  BBD1 谎

//huang4  BBCE 晃*

//hui1    BBD2 灰

//hui1    BBD3 挥

//hui1    BBD4 辉

//hui1    BBD5 徽

//hui1    BBD6 恢

//hui1    DAB6 诙

//hui1    DFD4 咴

//hui1    E3C4 隳

//hui1    E7F5 珲*

//hui1    EACD 晖

//hui1    F2B3 虺*

//hui1    F7E2 麾

//hui2    BBD7 蛔

//hui2    BBD8 回

//hui2    DCEE 茴

//hui2    E4A7 洄

//hui3    BBD9 毁

//hui3    BBDA 悔

//hui3    F2B3 虺*

//hui4    BBDB 慧

//hui4    BBDC 卉

//hui4    BBDD 惠

//hui4    BBDE 晦

//hui4    BBDF 贿

//hui4    BBE0 秽

//hui4    BBE1 会*

//hui4    BBE2 烩

//hui4    BBE3 汇

//hui4    BBE4 讳

//hui4    BBE5 诲

//hui4    BBE6 绘

//hui4    C0A3 溃*

//hui4    DCF6 荟

//hui4    DEA5 蕙

//hui4    DFDC 哕*

//hui4    E0B9 喙

//hui4    E4AB 浍*

//hui4    E5E7 彗

//hui4    E7C0 缋

//hui4    E8ED 桧*

//hui4    EDA3 恚

//hui4    F3B3 蟪

//hun1    BBE7 荤

//hun1    BBE8 昏

//hun1    BBE9 婚

//hun1    E3D4 阍

//hun2    BBEA 魂

//hun2    BBEB 浑

//hun2    BBEC 混*

//hun2    E2C6 馄

//hun2    E7F5 珲*

//hun4    BBEC 混*

//hun4    DABB 诨

//hun4    E4E3 溷

//huo1    BBED 豁*

//huo1    D8E5 劐

//huo1    DFAB 攉

//huo1    EFC1 锪

//huo1    F1EB 耠

//huo2    BACD 和*

//huo2    BBEE 活

//huo3    BBEF 伙

//huo3    BBF0 火

//huo3    E2B7 夥

//huo3    EED8 钬

//huo4    BACD 和*

//huo4    BBED 豁*

//huo4    BBF1 获

//huo4    BBF2 或

//huo4    BBF3 惑

//huo4    BBF4 霍

//huo4    BBF5 货

//huo4    BBF6 祸

//huo4    DEBD 藿

//huo4    E0EB 嚯

//huo4    EFEC 镬

//huo4    F3B6 蠖

//ji1     BBF7 击

//ji1     BBF8 圾

//ji1     BBF9 基

//ji1     BBFA 机

//ji1     BBFB 畸

//ji1     BBFC 稽*

//ji1     BBFD 积

//ji1     BBFE 箕

//ji1     BCA1 肌

//ji1     BCA2 饥

//ji1     BCA4 激

//ji1     BCA5 讥

//ji1     BCA6 鸡

//ji1     BCA7 姬

//ji1     BCA9 缉*

//ji1     BCB8 几*

//ji1     C6DA 期*

//ji1     C6E4 其*

//ji1     C6E6 奇*

//ji1     D8A2 丌

//ji1     D8C0 乩

//ji1     D8DE 剞

//ji1     DBD4 墼

//ji1     DCB8 芨

//ji1     DFB4 叽

//ji1     DFD2 咭

//ji1     DFF3 唧

//ji1     E5EC 屐

//ji1     E7DC 畿

//ji1     E7E1 玑

//ji1     EAE5 赍

//ji1     EAF7 犄

//ji1     ECB4 齑

//ji1     EDB6 矶

//ji1     EEBF 羁

//ji1     EFFA 嵇

//ji1     F3C7 笄

//ji1     F5D2 跻

//ji2     B8EF 革*

//ji2     BCAA 吉

//ji2     BCAB 极

//ji2     BCAC 棘

//ji2     BCAD 辑

//ji2     BCAE 籍

//ji2     BCAF 集

//ji2     BCB0 及

//ji2     BCB1 急

//ji2     BCB2 疾

//ji2     BCB3 汲

//ji2     BCB4 即

//ji2     BCB5 嫉

//ji2     BCB6 级

//ji2     BDE5 藉*

//ji2     D8BD 亟*

//ji2     D9A5 佶

//ji2     DAB5 诘*

//ji2     DDF0 蒺

//ji2     DEAA 蕺

//ji2     E1A7 岌

//ji2     E1D5 嵴

//ji2     E9AE 楫

//ji2     E9EA 殛

//ji2     EAAB 戢

//ji2     F1A4 瘠

//ji2     F3C5 笈

//ji3     B8F8 给*

//ji3     BCB7 挤

//ji3     BCB8 几*

//ji3     BCB9 脊

//ji3     BCBA 己

//ji3     BCC3 济*

//ji3     BCCD 纪*

//ji3     DEE1 掎

//ji3     EAAA 戟

//ji3     F2B1 虮

//ji3     F7E4 麂

//ji4     BCA3 迹

//ji4     BCA8 绩

//ji4     BCBB 蓟

//ji4     BCBC 技

//ji4     BCBD 冀

//ji4     BCBE 季

//ji4     BCBF 伎

//ji4     BCC0 祭

//ji4     BCC1 剂

//ji4     BCC2 悸

//ji4     BCC3 济*

//ji4     BCC4 寄

//ji4     BCC5 寂

//ji4     BCC6 计

//ji4     BCC7 记

//ji4     BCC8 既

//ji4     BCC9 忌

//ji4     BCCA 际

//ji4     BCCB 妓

//ji4     BCCC 继

//ji4     BCCD 纪*

//ji4     C6EB 齐*

//ji4     CFB5 系*

//ji4     D9CA 偈*

//ji4     DCC1 芰

//ji4     DCF9 荠*

//ji4     DFE2 哜

//ji4     E4A9 洎

//ji4     E6F7 骥

//ji4     EAE9 觊

//ji4     F0A2 稷

//ji4     F4DF 暨

//ji4     F5D5 跽

//ji4     F6AB 霁

//ji4     F6DD 鲚

//ji4     F6EA 鲫

//ji4     F7D9 髻

//jia1    BCCE 嘉

//jia1    BCCF 枷

//jia1    BCD0 夹*

//jia1    BCD1 佳

//jia1    BCD2 家*

//jia1    BCD3 加

//jia1    C7D1 茄*

//jia1    D0AE 挟*

//jia1    DDE7 葭

//jia1    E4A4 浃

//jia1    E5C8 迦

//jia1    E7EC 珈

//jia1    EFD8 镓

//jia1    F0E8 痂

//jia1    F3D5 笳

//jia1    F4C2 袈

//jia1    F5CA 跏

//jia2    BCD0 夹*

//jia2    BCD4 荚

//jia2    BCD5 颊

//jia2    DBA3 郏

//jia2    EAA9 戛

//jia2    EDA2 恝

//jia2    EEF2 铗

//jia2    F1CA 袷*

//jia2    F2CC 蛱

//jia3    BCD6 贾*

//jia3    BCD7 甲

//jia3    BCD8 钾

//jia3    BCD9 假*

//jia3    D8C5 嘏*

//jia3    E1B5 岬

//jia3    EBCE 胛

//jia3    F0FD 瘕

//jia4    BCD9 假*

//jia4    BCDA 稼

//jia4    BCDB 价*

//jia4    BCDC 架

//jia4    BCDD 驾

//jia4    BCDE 嫁

//jian1   BCDF 歼

//jian1   BCE0 监*

//jian1   BCE1 坚

//jian1   BCE2 尖

//jian1   BCE3 笺

//jian1   BCE4 间*

//jian1   BCE5 煎

//jian1   BCE6 兼

//jian1   BCE7 肩

//jian1   BCE8 艰

//jian1   BCE9 奸

//jian1   BCEA 缄

//jian1   BDA5 渐*

//jian1   BDA6 溅*

//jian1   C7B3 浅*

//jian1   DDD1 菅

//jian1   DDF3 蒹

//jian1   DEF6 搛

//jian1   E4D5 湔

//jian1   E7CC 缣

//jian1   EAA7 戋

//jian1   EAF9 犍*

//jian1   F0CF 鹣

//jian1   F6E4 鲣

//jian1   F7B5 鞯

//jian2   BCE0 监*

//jian3   BCEB 茧

//jian3   BCEC 检

//jian3   BCED 柬

//jian3   BCEE 碱

//jian3   BCEF 硷

//jian3   BCF0 拣

//jian3   BCF1 捡

//jian3   BCF2 简

//jian3   BCF3 俭

//jian3   BCF4 剪

//jian3   BCF5 减

//jian3   DAD9 谫

//jian3   E0EE 囝*

//jian3   E5BF 蹇

//jian3   E5C0 謇

//jian3   E8C5 枧

//jian3   EAAF 戬

//jian3   EDFA 睑

//jian3   EFB5 锏*

//jian3   F1D0 裥

//jian3   F3C8 笕

//jian3   F4E5 翦

//jian3   F5C2 趼

//jian4   BCE4 间*

//jian4   BCF6 荐

//jian4   BCF7 槛*

//jian4   BCF8 鉴

//jian4   BCF9 践

//jian4   BCFA 贱

//jian4   BCFB 见*

//jian4   BCFC 键

//jian4   BCFD 箭

//jian4   BCFE 件

//jian4   BDA1 健

//jian4   BDA2 舰

//jian4   BDA3 剑

//jian4   BDA4 饯

//jian4   BDA5 渐*

//jian4   BDA6 溅*

//jian4   BDA7 涧

//jian4   BDA8 建

//jian4   D9D4 僭

//jian4   DAC9 谏

//jian4   E9A5 楗

//jian4   EAF0 牮

//jian4   EBA6 毽

//jian4   EBEC 腱

//jian4   EFB5 锏*

//jian4   F5DD 踺

//jiang1  BDA9 僵

//jiang1  BDAA 姜

//jiang1  BDAB 将*

//jiang1  BDAC 浆*

//jiang1  BDAD 江

//jiang1  BDAE 疆

//jiang1  DCFC 茳

//jiang1  E7D6 缰

//jiang1  EDE4 礓

//jiang1  F4F8 豇

//jiang3  BDAF 蒋

//jiang3  BDB0 桨

//jiang3  BDB1 奖

//jiang3  BDB2 讲

//jiang3  F1F0 耩

//jiang4  BAE7 虹*

//jiang4  BDAB 将*

//jiang4  BDAC 浆*

//jiang4  BDB3 匠

//jiang4  BDB4 酱

//jiang4  BDB5 降*

//jiang4  C7BF 强*

//jiang4  E4AE 洚

//jiang4  E7AD 绛

//jiang4  EAF1 犟

//jiang4  F4DD 糨

//jiao1   BDB6 蕉

//jiao1   BDB7 椒

//jiao1   BDB8 礁

//jiao1   BDB9 焦

//jiao1   BDBA 胶

//jiao1   BDBB 交

//jiao1   BDBC 郊

//jiao1   BDBD 浇

//jiao1   BDBE 骄

//jiao1   BDBF 娇

//jiao1   BDCC 教*

//jiao1   D9D5 僬

//jiao1   DCB4 艽

//jiao1   DCFA 茭

//jiao1   E6AF 姣

//jiao1   F0D4 鹪

//jiao1   F2D4 蛟

//jiao1   F5D3 跤

//jiao1   F6DE 鲛

//jiao2   BDC0 嚼*

//jiao3   BDC1 搅

//jiao3   BDC2 铰

//jiao3   BDC3 矫

//jiao3   BDC4 侥*

//jiao3   BDC5 脚*

//jiao3   BDC6 狡

//jiao3   BDC7 角*

//jiao3   BDC8 饺

//jiao3   BDC9 缴*

//jiao3   BDCA 绞

//jiao3   BDCB 剿*

//jiao3   D9AE 佼

//jiao3   DED8 挢

//jiao3   E1E8 徼*

//jiao3   E4D0 湫*

//jiao3   EBB8 敫

//jiao3   F0A8 皎

//jiao4   BDC0 嚼*

//jiao4   BDCC 教*

//jiao4   BDCD 酵

//jiao4   BDCE 轿

//jiao4   BDCF 较

//jiao4   BDD0 叫

//jiao4   BDD1 窖

//jiao4   BEF5 觉*

//jiao4   D0A3 校*

//jiao4   E0DD 噍

//jiao4   E1BD 峤*

//jiao4   E1E8 徼*

//jiao4   ECDF 爝*

//jiao4   F5B4 醮

//jie1    BDD2 揭

//jie1    BDD3 接

//jie1    BDD4 皆

//jie1    BDD5 秸

//jie1    BDD6 街

//jie1    BDD7 阶

//jie1    BDDA 节*

//jie1    BDE1 结*

//jie1    BFAC 楷*

//jie1    E0AE 喈

//jie1    E0B5 嗟*

//jie1    F0DC 疖

//jie2    BDD8 截

//jie2    BDD9 劫

//jie2    BDDA 节*

//jie2    BDDB 桔*

//jie2    BDDC 杰

//jie2    BDDD 捷

//jie2    BDDE 睫

//jie2    BDDF 竭

//jie2    BDE0 洁

//jie2    BDE1 结*

//jie2    D9CA 偈*

//jie2    DAA6 讦

//jie2    DAB5 诘*

//jie2    DED7 拮

//jie2    E6BC 婕

//jie2    E6DD 孑

//jie2    E8EE 桀

//jie2    EDD9 碣

//jie2    F2A1 颉*

//jie2    F4C9 羯

//jie2    F6DA 鲒

//jie3    BDE2 解*

//jie3    BDE3 姐

//jie4    BCDB 价*

//jie4    BDE2 解*

//jie4    BDE4 戒

//jie4    BDE5 藉*

//jie4    BDE6 芥*

//jie4    BDE7 界

//jie4    BDE8 借

//jie4    BDE9 介

//jie4    BDEA 疥

//jie4    BDEB 诫

//jie4    BDEC 届

//jie4    F2BB 蚧

//jie4    F7BA 骱

//jie5    BCD2 家*

//jie5    BCDB 价*

//jin1    BDED 巾

//jin1    BDEE 筋

//jin1    BDEF 斤

//jin1    BDF0 金

//jin1    BDF1 今

//jin1    BDF2 津

//jin1    BDF3 襟

//jin1    BDFB 禁*

//jin1    F1C6 衿

//jin1    F1E6 矜*

//jin3    BDF4 紧

//jin3    BDF5 锦

//jin3    BDF6 仅*

//jin3    BDF7 谨

//jin3    BEA1 尽*

//jin3    DAE1 卺

//jin3    DDC0 堇

//jin3    E2CB 馑

//jin3    E2DB 廑*

//jin3    E8AA 瑾

//jin3    E9C8 槿

//jin4    BDF6 仅*

//jin4    BDF8 进

//jin4    BDF9 靳

//jin4    BDFA 晋

//jin4    BDFB 禁*

//jin4    BDFC 近

//jin4    BDFD 烬

//jin4    BDFE 浸

//jin4    BEA1 尽*

//jin4    BEA2 劲*

//jin4    DDA3 荩

//jin4    E0E4 噤

//jin4    E6A1 妗

//jin4    E7C6 缙

//jin4    EAE1 赆

//jin4    EAEE 觐

//jing1   BEA3 荆

//jing1   BEA4 兢

//jing1   BEA5 茎

//jing1   BEA6 睛

//jing1   BEA7 晶

//jing1   BEA8 鲸

//jing1   BEA9 京

//jing1   BEAA 惊

//jing1   BEAB 精

//jing1   BEAC 粳

//jing1   BEAD 经

//jing1   DDBC 菁

//jing1   E3FE 泾

//jing1   EBE6 腈

//jing1   ECBA 旌

//jing3   BEAE 井

//jing3   BEAF 警

//jing3   BEB0 景

//jing3   BEB1 颈*

//jing3   D8D9 刭

//jing3   D9D3 儆

//jing3   DAE5 阱

//jing3   E3BD 憬

//jing3   EBC2 肼

//jing4   BEA2 劲*

//jing4   BEB2 静

//jing4   BEB3 境

//jing4   BEB4 敬

//jing4   BEB5 镜

//jing4   BEB6 径

//jing4   BEB7 痉

//jing4   BEB8 靖

//jing4   BEB9 竟

//jing4   BEBA 竞

//jing4   BEBB 净

//jing4   E2B0 獍

//jing4   E5C9 迳

//jing4   E5F2 弪

//jing4   E6BA 婧

//jing4   EBD6 胫

//jing4   F6A6 靓*

//jiong1  ECE7 扃

//jiong3  BEBC 炯

//jiong3  BEBD 窘

//jiong3  E5C4 迥

//jiong3  EAC1 炅*

//jiu1    BEBE 揪

//jiu1    BEBF 究

//jiu1    BEC0 纠

//jiu1    E0B1 啾

//jiu1    E3CE 阄

//jiu1    F0AF 鸠

//jiu1    F4F1 赳

//jiu1    F7DD 鬏

//jiu3    BEC1 玖

//jiu3    BEC2 韭

//jiu3    BEC3 久

//jiu3    BEC4 灸

//jiu3    BEC5 九

//jiu3    BEC6 酒

//jiu4    BEC7 厩

//jiu4    BEC8 救

//jiu4    BEC9 旧

//jiu4    BECA 臼

//jiu4    BECB 舅

//jiu4    BECC 咎

//jiu4    BECD 就

//jiu4    BECE 疚

//jiu4    D9D6 僦

//jiu4    E8D1 柩

//jiu4    E8EA 桕

//jiu4    F0D5 鹫

//ju1     B3B5 车*

//ju1     BECF 鞠

//ju1     BED0 拘

//ju1     BED1 狙

//ju1     BED2 疽

//ju1     BED3 居

//ju1     BED4 驹

//ju1     BEDD 据*

//ju1     BEE2 锯*

//ju1     BEE3 俱*

//ju1     C7D2 且*

//ju1     DCDA 苴

//ju1     DEE4 掬

//ju1     E8A2 琚

//ju1     E9A7 椐

//ju1     EFB8 锔*

//ju1     F1D5 裾

//ju1     F4F2 趄*

//ju1     F6C2 雎

//ju1     F7B6 鞫

//ju2     BDDB 桔*

//ju2     BED5 菊

//ju2     BED6 局

//ju2     E9D9 橘

//ju2     EFB8 锔*

//ju3     B9F1 柜*

//ju3     BED7 咀*

//ju3     BED8 矩

//ju3     BED9 举

//ju3     BEDA 沮*

//ju3     DCEC 莒

//ju3     E8DB 枸*

//ju3     E9B0 榘

//ju3     E9B7 榉

//ju3     F5E1 踽

//ju3     F6B4 龃

//ju4     BEDA 沮*

//ju4     BEDB 聚

//ju4     BEDC 拒

//ju4     BEDD 据*

//ju4     BEDE 巨

//ju4     BEDF 具

//ju4     BEE0 距

//ju4     BEE1 踞

//ju4     BEE2 锯*

//ju4     BEE3 俱*

//ju4     BEE4 句*

//ju4     BEE5 惧

//ju4     BEE6 炬

//ju4     BEE7 剧

//ju4     D9C6 倨

//ju4     DAAA 讵

//ju4     DCC4 苣*

//ju4     E5E1 遽

//ju4     E5F0 屦

//ju4     EAF8 犋

//ju4     ECAB 飓

//ju4     EED2 钜

//ju4     F1C0 窭

//ju4     F5B6 醵

//ju4     F6C4 瞿*

//juan1   BEE8 捐

//juan1   BEE9 鹃

//juan1   BEEA 娟

//juan1   C8A6 圈*

//juan1   E4B8 涓

//juan1   EEC3 蠲

//juan1   EFD4 镌

//juan3   BEED 卷*

//juan3   EFC3 锩*

//juan4   BEEB 倦

//juan4   BEEC 眷

//juan4   BEED 卷*

//juan4   BEEE 绢

//juan4   C8A6 圈*

//juan4   DBB2 鄄

//juan4   E1FA 狷

//juan4   E8F0 桊

//juan4   F6C1 隽*

//jue1    BEEF 撅

//jue1    E0B5 嗟*

//jue1    E0D9 噘

//jue2    BDC0 嚼*

//jue2    BDC5 脚*

//jue2    BDC7 角*

//jue2    BEF0 攫

//jue2    BEF1 抉

//jue2    BEF2 掘

//jue2    BEF3 倔*

//jue2    BEF4 爵

//jue2    BEF5 觉*

//jue2    BEF6 决

//jue2    BEF7 诀

//jue2    BEF8 绝

//jue2    D8CA 厥

//jue2    D8E3 劂

//jue2    DADC 谲

//jue2    DBC7 矍

//jue2    DEA7 蕨

//jue2    E0E5 噱*

//jue2    E1C8 崛

//jue2    E2B1 獗

//jue2    E6DE 孓

//jue2    E7E5 珏

//jue2    E8F6 桷

//jue2    E9D3 橛

//jue2    ECDF 爝*

//jue2    EFE3 镢

//jue2    F5EA 蹶*

//jue2    F5FB 觖

//jue3    F5EA 蹶*

//jue4    BEF3 倔*

//jun1    B9EA 龟*

//jun1    BEF9 均

//jun1    BEFA 菌*

//jun1    BEFB 钧

//jun1    BEFC 军

//jun1    BEFD 君

//jun1    F1E4 皲

//jun1    F3DE 筠*

//jun1    F7E5 麇*

//jun4    BEFA 菌*

//jun4    BEFE 峻

//jun4    BFA1 俊

//jun4    BFA2 竣

//jun4    BFA3 浚*

//jun4    BFA4 郡

//jun4    BFA5 骏

//jun4    DEDC 捃

//jun4    F6C1 隽*

//ka1     BFA6 喀

//ka1     BFA7 咖*

//ka1     DFC7 咔*

//ka3     BFA8 卡*

//ka3     BFA9 咯*

//ka3     D8FB 佧

//ka3     DFC7 咔*

//ka3     EBCC 胩

//kai1    BFAA 开

//kai1    BFAB 揩

//kai1    EFB4 锎

//kai3    BFAC 楷*

//kai3    BFAD 凯

//kai3    BFAE 慨

//kai3    D8DC 剀

//kai3    DBEE 垲

//kai3    DDDC 蒈

//kai3    E2FD 恺

//kai3    EEF8 铠

//kai3    EFC7 锴

//kai4    E2E9 忾

//kan1    BFAF 刊

//kan1    BFB0 堪

//kan1    BFB1 勘

//kan1    BFB4 看*

//kan1    EAAC 戡

//kan1    EDE8 龛

//kan3    BCF7 槛*

//kan3    BFB2 坎

//kan3    BFB3 砍

//kan3    D9A9 侃

//kan3    DDA8 莰

//kan4    BFB4 看*

//kan4    C7B6 嵌*

//kan4    E3DB 阚*

//kan4    EEAB 瞰

//kang1   BFB5 康

//kang1   BFB6 慷

//kang1   BFB7 糠

//kang1   E3CA 闶

//kang2   BFB8 扛*

//kang4   BFB9 抗

//kang4   BFBA 亢

//kang4   BFBB 炕

//kang4   D8F8 伉

//kang4   EED6 钪

//kao1    E5EA 尻

//kao3    BFBC 考

//kao3    BFBD 拷

//kao3    BFBE 烤

//kao3    E8E0 栲

//kao4    BFBF 靠

//kao4    EAFB 犒

//kao4    EEED 铐

//ke1     BFC0 坷*

//ke1     BFC1 苛

//ke1     BFC2 柯

//ke1     BFC3 棵

//ke1     BFC4 磕

//ke1     BFC5 颗

//ke1     BFC6 科

//ke1     E0BE 嗑*

//ke1     E7E6 珂

//ke1     E9F0 轲

//ke1     EEA7 瞌

//ke1     EEDD 钶

//ke1     EFFD 稞

//ke1     F0E2 疴

//ke1     F1BD 窠

//ke1     F2A4 颏

//ke1     F2F2 蝌

//ke1     F7C1 髁

//ke2     BFC7 壳*

//ke2     BFC8 咳*

//ke3     BFC0 坷*

//ke3     BFC9 可*

//ke3     BFCA 渴

//ke3     E1B3 岢

//ke4     BFC9 可*

//ke4     BFCB 克

//ke4     BFCC 刻

//ke4     BFCD 客

//ke4     BFCE 课

//ke4     E0BE 嗑*

//ke4     E3A1 恪

//ke4     E4DB 溘

//ke4     E6EC 骒

//ke4     E7BC 缂

//ke4     EBB4 氪

//ke4     EFBE 锞

//ken3    BFCF 肯

//ken3    BFD0 啃

//ken3    BFD1 垦

//ken3    BFD2 恳

//ken3    F6B8 龈*

//ken4    F1CC 裉

//keng1   BFD3 坑

//keng1   BFD4 吭*

//keng1   EFAC 铿

//kong1   BFD5 空*

//kong1   D9C5 倥

//kong1   E1C7 崆

//kong1   F3ED 箜

//kong3   BFD6 恐

//kong3   BFD7 孔

//kong4   BFD5 空*

//kong4   BFD8 控

//kou1    BFD9 抠

//kou1    DCD2 芤

//kou1    EDEE 眍

//kou3    BFDA 口

//kou4    BFDB 扣

//kou4    BFDC 寇

//kou4    DEA2 蔻

//kou4    DFB5 叩

//kou4    F3D8 筘

//ku1     BFDD 枯

//ku1     BFDE 哭

//ku1     BFDF 窟

//ku1     D8DA 刳

//ku1     DCA5 堀

//ku1     F7BC 骷

//ku3     BFE0 苦

//ku4     BFE1 酷

//ku4     BFE2 库

//ku4     BFE3 裤

//ku4     E0B7 喾

//ku4     E7AB 绔

//kua1    BFE4 夸

//kua3    BFE5 垮

//kua3    D9A8 侉

//kua4    BFE6 挎

//kua4    BFE7 跨

//kua4    BFE8 胯

//kuai3   D8E1 蒯

//kuai4   BBE1 会*

//kuai4   BFE9 块

//kuai4   BFEA 筷

//kuai4   BFEB 侩

//kuai4   BFEC 快

//kuai4   DBA6 郐

//kuai4   DFE0 哙

//kuai4   E1F6 狯

//kuai4   E4AB 浍*

//kuai4   EBDA 脍

//kuan1   BFED 宽

//kuan1   F7C5 髋

//kuan3   BFEE 款

//kuang1  BFEF 匡

//kuang1  BFF0 筐

//kuang1  DAB2 诓

//kuang1  DFD1 哐

//kuang2  BFF1 狂

//kuang2  DABF 诳

//kuang3  DEC5 夼

//kuang4  BFF2 框

//kuang4  BFF3 矿

//kuang4  BFF4 眶

//kuang4  BFF5 旷

//kuang4  BFF6 况

//kuang4  DAF7 邝

//kuang4  DBDB 圹

//kuang4  E6FE 纩

//kuang4  EADC 贶

//kui1    BFF7 亏

//kui1    BFF8 盔

//kui1    BFF9 岿

//kui1    BFFA 窥

//kui1    E3A6 悝

//kui2    BFFB 葵

//kui2    BFFC 奎

//kui2    BFFD 魁

//kui2    D8B8 馗

//kui2    D9E7 夔

//kui2    DAF3 隗*

//kui2    DEF1 揆

//kui2    E0AD 喹

//kui2    E5D3 逵

//kui2    EAD2 暌

//kui2    EEA5 睽

//kui2    F2F1 蝰

//kui3    BFFE 傀*

//kui3    F5CD 跬

//kui4    C0A1 馈

//kui4    C0A2 愧

//kui4    C0A3 溃*

//kui4    D8D1 匮

//kui4    DDDE 蒉

//kui4    E0B0 喟

//kui4    E3B4 愦

//kui4    F1F9 聩

//kui4    F3F1 篑

//kun1    C0A4 坤

//kun1    C0A5 昆

//kun1    E7FB 琨

//kun1    EFBF 锟

//kun1    F5AB 醌

//kun1    F6EF 鲲

//kun1    F7D5 髡

//kun3    C0A6 捆

//kun3    E3A7 悃

//kun3    E3CD 阃

//kun4    C0A7 困

//kuo4    C0A8 括*

//kuo4    C0A9 扩

//kuo4    C0AA 廓

//kuo4    C0AB 阔

//kuo4    CACA 适*

//kuo4    F2D2 蛞

//la1     C0AC 垃

//la1     C0AD 拉*

//la1     C0AE 喇*

//la1     C0AE 喇*

//la1     C0B2 啦*

//la1     E5E5 邋

//la2     C0AD 拉*

//la2     EAB9 旯

//la2     EDC7 砬

//la3     C0AE 喇*

//la4     C0AF 蜡*

//la4     C0B0 腊*

//la4     C0B1 辣

//la4     C2E4 落*

//la4     D8DD 剌

//la4     F0F8 瘌

//la5     C0B2 啦*

//lai2    C0B3 莱

//lai2    C0B4 来

//lai2    E1C1 崃

//lai2    E1E2 徕

//lai2    E4B5 涞

//lai2    EFAA 铼

//lai4    C0B5 赖

//lai4    E4FE 濑

//lai4    EAE3 赉

//lai4    EDF9 睐

//lai4    F1AE 癞

//lai4    F4A5 籁

//lan2    C0B6 蓝

//lan2    C0B7 婪

//lan2    C0B8 栏

//lan2    C0B9 拦

//lan2    C0BA 篮

//lan2    C0BB 阑

//lan2    C0BC 兰

//lan2    C0BD 澜

//lan2    C0BE 谰

//lan2    E1B0 岚

//lan2    ECB5 斓

//lan2    EFE7 镧

//lan2    F1DC 褴

//lan3    C0BF 揽

//lan3    C0C0 览

//lan3    C0C1 懒

//lan3    C0C2 缆

//lan3    E4ED 漤

//lan3    E9AD 榄

//lan3    EEBD 罱

//lan4    C0C3 烂

//lan4    C0C4 滥

//lang1   E0A5 啷

//lang2   C0C5 琅

//lang2   C0C6 榔

//lang2   C0C7 狼

//lang2   C0C8 廊

//lang2   C0C9 郎

//lang2   E3CF 阆*

//lang2   EFB6 锒

//lang2   EFFC 稂

//lang2   F2EB 螂

//lang3   C0CA 朗

//lang4   C0CB 浪

//lang4   DDB9 莨

//lang4   DDF5 蒗

//lang4   E3CF 阆*

//lao1    C0CC 捞

//lao2    C0CD 劳

//lao2    C0CE 牢

//lao2    DFEB 唠*

//lao2    E1C0 崂

//lao2    EFA9 铹

//lao2    F0EC 痨

//lao2    F5B2 醪

//lao3    C0CF 老

//lao3    C0D0 佬

//lao3    C0D1 姥*

//lao3    C1CA 潦*

//lao3    E8E1 栳

//lao3    EEEE 铑

//lao4    C0D2 酪

//lao4    C0D3 烙*

//lao4    C0D4 涝

//lao4    C2E4 落*

//lao4    C2E7 络*

//lao4    DFEB 唠*

//lao4    F1EC 耢

//le1     C0DF 肋*

//le4     C0D5 勒*

//le4     C0D6 乐*

//le4     D8EC 仂

//le4     DFB7 叻

//le4     E3EE 泐

//le4     F7A6 鳓

//le5     C1CB 了*

//lei1    C0D5 勒*

//lei2    C0D7 雷

//lei2    C0D8 镭

//lei2    C0DB 累*

//lei2    C0DE 擂*

//lei2    D9FA 羸

//lei2    E6D0 嫘

//lei2    E7D0 缧

//lei2    E9DB 檑

//lei3    C0D9 蕾

//lei3    C0DA 磊

//lei3    C0DB 累*

//lei3    C0DC 儡

//lei3    C0DD 垒

//lei3    DAB3 诔

//lei3    F1E7 耒

//lei4    C0DB 累*

//lei4    C0DE 擂*

//lei4    C0DF 肋*

//lei4    C0E0 类

//lei4    C0E1 泪

//lei4    F5AA 酹

//lei5    E0CF 嘞

//leng1   C0E2 棱*

//leng2   C0E2 棱*

//leng2   C0E3 楞

//leng2   DCA8 塄

//leng3   C0E4 冷

//leng4   E3B6 愣

//li1     C1A8 哩*

//li2     C0E5 厘

//li2     C0E6 梨

//li2     C0E7 犁

//li2     C0E8 黎

//li2     C0E9 篱

//li2     C0EA 狸

//li2     C0EB 离

//li2     C0EC 漓

//li2     C0F6 丽*

//li2     C1A7 璃

//li2     DDF1 蓠

//li2     DEBC 藜

//li2     E0AC 喱

//li2     E6CB 嫠

//li2     E6EA 骊

//li2     E7CA 缡

//li2     EEBE 罹

//li2     F0BF 鹂

//li2     F2DB 蜊

//li2     F3BB 蠡*

//li2     F6E2 鲡

//li2     F7F3 黧

//li3     C0ED 理

//li3     C0EE 李

//li3     C0EF 里

//li3     C0F0 鲤

//li3     C0F1 礼

//li3     C1A8 哩*

//li3     D9B5 俚

//li3     E5A2 澧

//li3     E5CE 逦

//li3     E6B2 娌

//li3     EFAE 锂

//li3     F3BB 蠡*

//li3     F5B7 醴

//li3     F7AF 鳢

//li4     C0F2 莉

//li4     C0F3 荔

//li4     C0F4 吏

//li4     C0F5 栗

//li4     C0F6 丽*

//li4     C0F7 厉

//li4     C0F8 励

//li4     C0F9 砾

//li4     C0FA 历

//li4     C0FB 利

//li4     C0FC 傈

//li4     C0FD 例

//li4     C0FE 俐

//li4     C1A1 痢

//li4     C1A2 立

//li4     C1A3 粒

//li4     C1A4 沥

//li4     C1A5 隶

//li4     C1A6 力

//li4     D8AA 鬲*

//li4     D9B3 俪

//li4     DBAA 郦

//li4     DBDE 坜

//li4     DCC2 苈

//li4     DDB0 莅

//li4     DFBF 呖

//li4     E0A6 唳

//li4     E1FB 猁

//li4     E4E0 溧

//li4     E8C0 枥

//li4     E8DD 栎*

//li4     E9F6 轹

//li4     ECE5 戾

//li4     EDC2 砺

//li4     EEBA 詈

//li4     F0DD 疠

//li4     F0DF 疬

//li4     F2C3 蛎

//li4     F3D2 笠

//li4     F3F6 篥

//li4     F4CF 粝

//li4     F5C8 跞*

//li4     F6A8 雳

//li5     C1A8 哩*

//lia3    C1A9 俩*

//lian2   C1AA 联

//lian2   C1AB 莲

//lian2   C1AC 连

//lian2   C1AD 镰

//lian2   C1AE 廉

//lian2   C1AF 怜

//lian2   C1B0 涟

//lian2   C1B1 帘

//lian2   DEC6 奁

//lian2   E5A5 濂

//lian2   ECA1 臁

//lian2   F1CD 裢

//lian2   F3B9 蠊

//lian2   F6E3 鲢

//lian3   C1B2 敛

//lian3   C1B3 脸

//lian3   DDFC 蔹

//lian3   E7F6 琏

//lian3   F1CF 裣

//lian4   C1B4 链

//lian4   C1B5 恋

//lian4   C1B6 炼

//lian4   C1B7 练

//lian4   E4F2 潋

//lian4   E9AC 楝

//lian4   E9E7 殓

//liang2  C1B8 粮

//liang2  C1B9 凉*

//liang2  C1BA 梁

//liang2  C1BB 粱

//liang2  C1BC 良

//liang2  C1BF 量*

//liang2  DCAE 墚

//liang2  E9A3 椋

//liang2  F5D4 踉*

//liang3  C1A9 俩*

//liang3  C1BD 两

//liang3  F7CB 魉

//liang4  C1B9 凉*

//liang4  C1BE 辆

//liang4  C1BF 量*

//liang4  C1C0 晾

//liang4  C1C1 亮

//liang4  C1C2 谅

//liang4  F5D4 踉*

//liang4  F6A6 靓*

//liao1   C1C3 撩*

//liao2   C1C3 撩*

//liao2   C1C4 聊

//liao2   C1C5 僚

//liao2   C1C6 疗

//liao2   C1C7 燎*

//liao2   C1C8 寥

//liao2   C1C9 辽

//liao2   C1CA 潦*

//liao2   E0DA 嘹

//liao2   E2B2 獠

//liao2   E5BC 寮

//liao2   E7D4 缭

//liao2   F0D3 鹩

//liao3   C1C7 燎*

//liao3   C1CB 了*

//liao3   DEA4 蓼

//liao3   EEC9 钌*

//liao4   C1CC 撂

//liao4   C1CD 镣

//liao4   C1CE 廖

//liao4   C1CF 料

//liao4   DECD 尥

//liao4   EEC9 钌*

//lie1    DFD6 咧*

//lie3    C1D1 裂*

//lie3    DFD6 咧*

//lie4    C1D0 列

//lie4    C1D1 裂*

//lie4    C1D2 烈

//lie4    C1D3 劣

//lie4    C1D4 猎

//lie4    D9FD 冽

//lie4    DBF8 埒

//lie4    DEE6 捩

//lie4    E4A3 洌

//lie4    F4F3 趔

//lie4    F5F1 躐

//lie4    F7E0 鬣

//lie5    DFD6 咧*

//lin1    C1E0 拎

//lin2    C1D5 琳

//lin2    C1D6 林

//lin2    C1D7 磷

//lin2    C1D8 霖

//lin2    C1D9 临

//lin2    C1DA 邻

//lin2    C1DB 鳞

//lin2    C1DC 淋*

//lin2    DFF8 啉

//lin2    E1D7 嶙

//lin2    E5E0 遴

//lin2    EAA5 辚

//lin2    EEAC 瞵

//lin2    F4D4 粼

//lin2    F7EB 麟

//lin3    C1DD 凛

//lin3    E2DE 廪

//lin3    E3C1 懔

//lin3    E9DD 檩

//lin4    C1DC 淋*

//lin4    C1DE 赁

//lin4    C1DF 吝

//lin4    DDFE 蔺

//lin4    ECA2 膦

//lin4    F5EF 躏

//ling2   C0E2 棱*

//ling2   C1E1 玲

//ling2   C1E2 菱

//ling2   C1E3 零

//ling2   C1E4 龄

//ling2   C1E5 铃

//ling2   C1E6 伶

//ling2   C1E7 羚

//ling2   C1E8 凌

//ling2   C1E9 灵

//ling2   C1EA 陵

//ling2   C1EE 令*

//ling2   DBB9 酃

//ling2   DCDF 苓

//ling2   E0F2 囹

//ling2   E3F6 泠

//ling2   E7B1 绫

//ling2   E8DA 柃

//ling2   E8F9 棂

//ling2   EAB2 瓴

//ling2   F1F6 聆

//ling2   F2C8 蛉

//ling2   F4E1 翎

//ling2   F6EC 鲮

//ling3   C1EB 岭

//ling3   C1EC 领

//ling3   C1EE 令*

//ling4   C1ED 另

//ling4   C1EE 令*

//ling4   DFCA 呤

//liu1    C1EF 溜*

//liu1    ECD6 熘

//liu2    C1F0 琉

//liu2    C1F1 榴

//liu2    C1F2 硫

//liu2    C1F3 馏*

//liu2    C1F4 留

//liu2    C1F5 刘

//liu2    C1F6 瘤

//liu2    C1F7 流

//liu2    E4AF 浏

//liu2    E5DE 遛*

//liu2    E6F2 骝

//liu2    ECBC 旒

//liu2    EFD6 镏*

//liu2    F6CC 鎏

//liu3    C1F8 柳

//liu3    E7B8 绺

//liu3    EFB3 锍

//liu4    C1EF 溜*

//liu4    C1F3 馏*

//liu4    C1F9 六*

//liu4    C2B5 碌*

//liu4    C2BD 陆*

//liu4    E5DE 遛*

//liu4    EFD6 镏*

//liu4    F0D2 鹨

//lo5     BFA9 咯*

//long1   C2A1 隆*

//long2   C1FA 龙

//long2   C1FB 聋

//long2   C1FC 咙

//long2   C1FD 笼*

//long2   C1FE 窿

//long2   C2A1 隆*

//long2   DCD7 茏

//long2   E3F1 泷*

//long2   E7E7 珑

//long2   E8D0 栊

//long2   EBCA 胧

//long2   EDC3 砻

//long2   F1AA 癃

//long3   C1FD 笼*

//long3   C2A2 垄

//long3   C2A3 拢

//long3   C2A4 陇

//long3   DBE2 垅

//long4   C5AA 弄*

//lou1    C2A7 搂*

//lou2    C2A5 楼

//lou2    C2A6 娄

//lou2    D9CD 偻*

//lou2    DDE4 蒌

//lou2    E0B6 喽*

//lou2    F1EF 耧

//lou2    F2F7 蝼

//lou2    F7C3 髅

//lou3    C2A7 搂*

//lou3    C2A8 篓

//lou3    E1D0 嵝

//lou4    C2A9 漏

//lou4    C2AA 陋

//lou4    C2B6 露*

//lou4    EFCE 镂

//lou4    F0FC 瘘

//lou5    E0B6 喽*

//lu1     DFA3 撸

//lu1     E0E0 噜

//lu2     C2AB 芦

//lu2     C2AC 卢

//lu2     C2AD 颅

//lu2     C2AE 庐

//lu2     C2AF 炉

//lu2     DBE4 垆

//lu2     E3F2 泸

//lu2     E8D3 栌

//lu2     E9F1 轳

//lu2     EBCD 胪

//lu2     F0B5 鸬

//lu2     F4B5 舻

//lu2     F6D4 鲈

//lu3     C2B0 掳

//lu3     C2B1 卤

//lu3     C2B2 虏

//lu3     C2B3 鲁

//lu3     E9D6 橹

//lu3     EFE5 镥

//lu4     C1F9 六*

//lu4     C2B4 麓

//lu4     C2B5 碌*

//lu4     C2B6 露*

//lu4     C2B7 路

//lu4     C2B8 赂

//lu4     C2B9 鹿

//lu4     C2BA 潞

//lu4     C2BB 禄

//lu4     C2BC 录

//lu4     C2BD 陆*

//lu4     C2BE 戮

//lu4     C2CC 绿*

//lu4     E4CB 渌

//lu4     E4F5 漉

//lu4     E5D6 逯

//lu4     E8B4 璐

//lu4     E9FB 辂

//lu4     EAA4 辘

//lu4     F0D8 鹭

//lu4     F3FC 簏

//lu5     EBAA 氇

//lu:2    C2BF 驴

//lu:2    E3CC 闾

//lu:2    E9B5 榈

//lu:3    C2C0 吕

//lu:3    C2C1 铝

//lu:3    C2C2 侣

//lu:3    C2C3 旅

//lu:3    C2C4 履

//lu:3    C2C5 屡

//lu:3    C2C6 缕

//lu:3    D9CD 偻*

//lu:3    DEDB 捋*

//lu:3    EBF6 膂

//lu:3    EFF9 稆

//lu:3    F1DA 褛

//lu:4    C2C7 虑

//lu:4    C2C8 氯

//lu:4    C2C9 律

//lu:4    C2CA 率*

//lu:4    C2CB 滤

//lu:4    C2CC 绿*

//luan2   C2CD 峦

//luan2   C2CE 挛

//luan2   C2CF 孪

//luan2   C2D0 滦

//luan2   D9F5 脔

//luan2   E6AE 娈

//luan2   E8EF 栾

//luan2   F0BD 鸾

//luan2   F6C7 銮

//luan3   C2D1 卵

//luan4   C2D2 乱

//lue:3   C2D3 掠*

//lue:4   C2D3 掠*

//lue:4   C2D4 略

//lue:4   EFB2 锊

//lun1    C2D5 抡*

//lun2    C2D5 抡*

//lun2    C2D6 轮

//lun2    C2D7 伦

//lun2    C2D8 仑

//lun2    C2D9 沦

//lun2    C2DA 纶*

//lun2    C2DB 论*

//lun2    E0F0 囵

//lun4    C2DB 论*

//luo1    DEDB 捋*

//luo2    C2DC 萝

//luo2    C2DD 螺

//luo2    C2DE 罗

//luo2    C2DF 逻

//luo2    C2E0 锣

//luo2    C2E1 箩

//luo2    C2E2 骡

//luo2    E2A4 猡

//luo2    E9A1 椤

//luo2    EBE1 脶

//luo2    EFDD 镙

//luo3    C2E3 裸

//luo3    D9C0 倮

//luo3    D9F9 蠃

//luo3    F1A7 瘰

//luo4    C0D3 烙*

//luo4    C2E4 落*

//luo4    C2E5 洛

//luo4    C2E6 骆

//luo4    C2E7 络*

//luo4    DCFD 荦

//luo4    DEFB 摞

//luo4    E3F8 泺*

//luo4    E4F0 漯*

//luo4    E7F3 珞

//luo4    EDD1 硌*

//luo4    F5C8 跞*

//luo4    F6C3 雒

//m2      DFBC 呒

//ma1     C2E8 妈

//ma1     C2E9 麻*

//ma1     C2EC 蚂*

//ma1     C4A6 摩*

//ma1     C4A8 抹*

//ma2     C2E9 麻*

//ma2     C2F0 吗*

//ma2     F3A1 蟆

//ma3     C2EA 玛

//ma3     C2EB 码

//ma3     C2EC 蚂*

//ma3     C2ED 马

//ma3     C2F0 吗*

//ma3     E1EF 犸

//ma4     C2EC 蚂*

//ma4     C2EE 骂

//ma4     DFE9 唛

//ma4     E8BF 杩

//ma5     C2EF 嘛

//ma5     C2F0 吗*

//ma5     C3B4 么*

//mai2    C2F1 埋*

//mai2    F6B2 霾

//mai3    C2F2 买

//mai3    DDA4 荬

//mai4    C2F3 麦

//mai4    C2F4 卖

//mai4    C2F5 迈

//mai4    C2F6 脉*

//mai4    DBBD 劢

//man1    F2A9 颟

//man2    C2F1 埋*

//man2    C2F7 瞒

//man2    C2F8 馒

//man2    C2F9 蛮

//man2    C2FB 蔓*

//man2    C3A1 谩*

//man2    F7A9 鳗

//man2    F7B4 鞔

//man3    C2FA 满

//man3    F2FD 螨

//man4    C2FB 蔓*

//man4    C2FC 曼

//man4    C2FD 慢

//man4    C2FE 漫

//man4    C3A1 谩*

//man4    DCAC 墁

//man4    E1A3 幔

//man4    E7CF 缦

//man4    ECD7 熳

//man4    EFDC 镘

//mang2   C3A2 芒

//mang2   C3A3 茫

//mang2   C3A4 盲

//mang2   C3A5 氓*

//mang2   C3A6 忙

//mang2   DAF8 邙

//mang2   EDCB 硭

//mang3   C3A7 莽

//mang3   E4DD 漭

//mang3   F2FE 蟒

//mao1    C3A8 猫*

//mao2    C3A8 猫*

//mao2    C3A9 茅

//mao2    C3AA 锚

//mao2    C3AB 毛

//mao2    C3AC 矛

//mao2    DCE2 茆

//mao2    EAF3 牦

//mao2    ECB8 旄

//mao2    F2FA 蝥

//mao2    F3B1 蟊

//mao2    F7D6 髦

//mao3    C3AD 铆

//mao3    C3AE 卯

//mao3    E1B9 峁

//mao3    E3F7 泖

//mao3    EAC4 昴

//mao4    C3AF 茂

//mao4    C3B0 冒*

//mao4    C3B1 帽

//mao4    C3B2 貌

//mao4    C3B3 贸

//mao4    D9F3 袤

//mao4    E8A3 瑁

//mao4    EBA3 耄

//mao4    EDAE 懋

//mao4    EEA6 瞀

//me5     C3B4 么*

//me5     F7E1 麽*

//mei2    C3B5 玫

//mei2    C3B6 枚

//mei2    C3B7 梅

//mei2    C3B8 酶

//mei2    C3B9 霉

//mei2    C3BA 煤

//mei2    C3BB 没*

//mei2    C3BC 眉

//mei2    C3BD 媒

//mei2    C3D3 糜*

//mei2    DDAE 莓

//mei2    E1D2 嵋

//mei2    E2AD 猸

//mei2    E4D8 湄

//mei2    E9B9 楣

//mei2    EFD1 镅

//mei2    F0CC 鹛

//mei3    C3BE 镁

//mei3    C3BF 每

//mei3    C3C0 美

//mei3    E4BC 浼

//mei4    C3C1 昧

//mei4    C3C2 寐

//mei4    C3C3 妹

//mei4    C3C4 媚

//mei4    C3D5 谜*

//mei4    F1C7 袂

//mei4    F7C8 魅

//men1    C3C6 闷*

//men2    C3C5 门

//men2    DED1 扪

//men2    EECD 钔

//men4    C3C6 闷*

//men4    ECCB 焖

//men4    EDAF 懑

//men5    C3C7 们

//meng1   C3C9 蒙*

//meng2   C3A5 氓*

//meng2   C3C8 萌

//meng2   C3C9 蒙*

//meng2   C3CA 檬

//meng2   C3CB 盟

//meng2   DDF9 甍

//meng2   DEAB 瞢

//meng2   EBFC 朦

//meng2   EDE6 礞

//meng2   F2B5 虻

//meng2   F4BF 艨

//meng3   C3C9 蒙*

//meng3   C3CC 锰

//meng3   C3CD 猛

//meng3   DBC2 勐

//meng3   E3C2 懵

//meng3   F2EC 蜢

//meng3   F3B7 蠓

//meng3   F4BB 艋

//meng4   C3CE 梦

//meng4   C3CF 孟

//mi1     C3D0 眯*

//mi1     DFE4 咪

//mi2     C3D0 眯*

//mi2     C3D1 醚

//mi2     C3D2 靡*

//mi2     C3D3 糜*

//mi2     C3D4 迷

//mi2     C3D5 谜*

//mi2     C3D6 弥

//mi2     DEC2 蘼

//mi2     E2A8 猕

//mi2     ECF2 祢

//mi2     F7E3 縻

//mi2     F7E7 麋

//mi3     C3D2 靡*

//mi3     C3D7 米

//mi3     D8C2 芈

//mi3     E5F4 弭

//mi3     EBDF 脒

//mi3     F4CD 敉

//mi4     C3D8 秘*

//mi4     C3D9 觅

//mi4     C3DA 泌*

//mi4     C3DB 蜜

//mi4     C3DC 密

//mi4     C3DD 幂

//mi4     DAD7 谧

//mi4     E0D7 嘧

//mi4     E3E8 汨

//mi4     E5B5 宓

//mi4     F4E9 糸

//mian2   C3DE 棉

//mian2   C3DF 眠

//mian2   C3E0 绵

//mian3   C3E1 冕

//mian3   C3E2 免

//mian3   C3E3 勉

//mian3   C3E4 娩

//mian3   C3E5 缅

//mian3   E3E6 沔

//mian3   E4C5 渑*

//mian3   E4CF 湎

//mian3   EBEF 腼

//mian3   EDED 眄*

//mian3   F6BC 黾*

//mian4   C3E6 面

//mian4   EDED 眄*

//miao1   DFF7 喵

//miao2   C3E7 苗

//miao2   C3E8 描

//miao2   C3E9 瞄

//miao2   F0C5 鹋

//miao3   C3EA 藐

//miao3   C3EB 秒

//miao3   C3EC 渺

//miao3   E5E3 邈

//miao3   E7BF 缈

//miao3   E8C2 杪

//miao3   EDB5 淼

//miao3   EDF0 眇

//miao4   C3ED 庙

//miao4   C3EE 妙

//miao4   E7D1 缪*

//mie1    D8BF 乜*

//mie1    DFE3 咩

//mie4    C3EF 蔑

//mie4    C3F0 灭

//mie4    F3BA 蠛

//mie4    F3FA 篾

//min2    C3F1 民

//min2    DCE5 苠

//min2    E1BA 岷

//min2    E7C5 缗

//min2    E7E4 玟

//min2    E7EB 珉

//min3    C3F2 抿

//min3    C3F3 皿

//min3    C3F4 敏

//min3    C3F5 悯

//min3    C3F6 闽

//min3    E3C9 闵

//min3    E3FD 泯

//min3    EDAA 愍

//min3    F6BC 黾*

//min3    F7AA 鳘

//ming2   C3F7 明

//ming2   C3F8 螟

//ming2   C3F9 鸣

//ming2   C3FA 铭

//ming2   C3FB 名

//ming2   DAA4 冥

//ming2   DCF8 茗

//ming2   E4E9 溟

//ming2   EAD4 暝

//ming2   EEA8 瞑

//ming3   F5A4 酩

//ming4   C3FC 命

//miu4    C3FD 谬

//miu4    E7D1 缪*

//mo1     C3FE 摸

//mo2     C4A1 摹

//mo2     C4A2 蘑

//mo2     C4A3 模*

//mo2     C4A4 膜

//mo2     C4A5 磨*

//mo2     C4A6 摩*

//mo2     C4A7 魔

//mo2     CEDE 无*

//mo2     DAD3 谟

//mo2     E2C9 馍

//mo2     E6C6 嫫

//mo2     E6D6 嬷

//mo2     F7E1 麽*

//mo3     C4A8 抹*

//mo4     BAD9 嘿*

//mo4     C2F6 脉*

//mo4     C3B0 冒*

//mo4     C3BB 没*

//mo4     C4A5 磨*

//mo4     C4A8 抹*

//mo4     C4A9 末

//mo4     C4AA 莫

//mo4     C4AB 墨

//mo4     C4AC 默

//mo4     C4AD 沫

//mo4     C4AE 漠

//mo4     C4AF 寞

//mo4     C4B0 陌

//mo4     CDF2 万*

//mo4     DCD4 茉

//mo4     DDEB 蓦

//mo4     E9E2 殁

//mo4     EFD2 镆

//mo4     EFF7 秣

//mo4     F1A2 瘼

//mo4     F1F2 耱

//mo4     F5F6 貊

//mo4     F5F8 貘

//mou1    DFE8 哞

//mou2    C4B1 谋

//mou2    C4B2 牟*

//mou2    D9B0 侔

//mou2    E7D1 缪*

//mou2    EDF8 眸

//mou2    F2D6 蛑

//mou2    F6CA 鍪

//mou3    C4B3 某

//mu2     C4A3 模*

//mu2     EBA4 毪

//mu3     C0D1 姥*

//mu3     C4B4 拇

//mu3     C4B5 牡

//mu3     C4B6 亩

//mu3     C4B7 姆

//mu3     C4B8 母

//mu3     DBE9 坶

//mu4     C4B2 牟*

//mu4     C4B9 墓

//mu4     C4BA 暮

//mu4     C4BB 幕

//mu4     C4BC 募

//mu4     C4BD 慕

//mu4     C4BE 木

//mu4     C4BF 目

//mu4     C4C0 睦

//mu4     C4C1 牧

//mu4     C4C2 穆

//mu4     D8EF 仫

//mu4     DCD9 苜

//mu4     E3E5 沐

//mu4     EEE2 钼

//n2      E0C5 嗯*

//n3      E0C5 嗯*

//n4      E0C5 嗯*

//na1     C4C7 那*

//na1     C4CF 南*

//na2     C4C3 拿

//na2     EFD5 镎

//na3     C4C4 哪*

//na4     C4C5 呐

//na4     C4C6 钠

//na4     C4C7 那*

//na4     C4C8 娜*

//na4     C4C9 纳

//na4     DEE0 捺

//na4     EBC7 肭

//na4     F1C4 衲

//na5     C4C4 哪*

//nai3    C4CA 氖

//nai3    C4CB 乃

//nai3    C4CC 奶

//nai3    DCB5 艿

//nai4    C4CD 耐

//nai4    C4CE 奈

//nai4    D8BE 鼐

//nai4    D9A6 佴*

//nai4    DDC1 萘

//nai4    E8CD 柰

//nan1    E0EE 囝*

//nan1    E0EF 囡

//nan2    C4CF 南*

//nan2    C4D0 男

//nan2    C4D1 难*

//nan2    E0AB 喃

//nan2    E9AA 楠

//nan3    EBEE 腩

//nan3    F2EF 蝻

//nan3    F4F6 赧

//nan4    C4D1 难*

//nang1   C4D2 囊*

//nang1   E0EC 囔

//nang2   C4D2 囊*

//nang2   E2CE 馕*

//nang3   DFAD 攮

//nang3   E2CE 馕*

//nang3   EAD9 曩

//nao1    D8AB 孬

//nao2    C4D3 挠

//nao2    DFCE 呶

//nao2    E2AE 猱

//nao2    EDD0 硇

//nao2    EEF3 铙

//nao2    F2CD 蛲

//nao3    C4D4 脑

//nao3    C4D5 恼

//nao3    DBF1 垴

//nao3    E8A7 瑙

//nao4    C4D6 闹

//nao4    C4D7 淖

//ne2     C4C4 哪*

//ne4     DAAB 讷

//ne5     C4D8 呢*

//nei3    C4C4 哪*

//nei3    C4D9 馁

//nei4    C4C7 那*

//nei4    C4DA 内

//nen4    C4DB 嫩

//nen4    EDA5 恁

//neng2   C4DC 能

//ng2     DFED 唔*

//ng2     E0C5 嗯*

//ng3     E0C5 嗯*

//ng4     E0C5 嗯*

//ni1     C4DD 妮

//ni2     C4D8 呢*

//ni2     C4DE 霓

//ni2     C4DF 倪

//ni2     C4E0 泥*

//ni2     C4E1 尼

//ni2     DBE8 坭

//ni2     E2A5 猊

//ni2     E2F5 怩

//ni2     EEEA 铌

//ni2     F6F2 鲵

//ni3     C4E2 拟

//ni3     C4E3 你

//ni3     ECBB 旎

//ni4     C4E0 泥*

//ni4     C4E4 匿

//ni4     C4E5 腻

//ni4     C4E6 逆

//ni4     C4E7 溺*

//ni4     D9A3 伲

//ni4     EAC7 昵

//ni4     EDFE 睨

//nian1   C4E8 蔫

//nian1   C4E9 拈

//nian2   C4EA 年

//nian2   D5B3 粘*

//nian2   F0A4 黏

//nian2   F6D3 鲇

//nian2   F6F3 鲶

//nian3   C4EB 碾

//nian3   C4EC 撵

//nian3   C4ED 捻

//nian3   E9FD 辇

//nian4   C4EE 念

//nian4   D8A5 廿

//nian4   DBFE 埝

//niang2  C4EF 娘

//niang4  C4F0 酿

//niao3   C4F1 鸟*

//niao3   DCE0 茑

//niao3   E6D5 嬲

//niao3   F4C1 袅

//niao4   C4E7 溺*

//niao4   C4F2 尿*

//niao4   EBE5 脲

//nie1    C4F3 捏

//nie4    C4F4 聂

//nie4    C4F5 孽

//nie4    C4F6 啮

//nie4    C4F7 镊

//nie4    C4F8 镍

//nie4    C4F9 涅

//nie4    D8BF 乜*

//nie4    DAED 陧

//nie4    DEC1 蘖

//nie4    E0BF 嗫

//nie4    F2A8 颞

//nie4    F4AB 臬

//nie4    F5E6 蹑

//nin2    C4FA 您

//ning2   C4FB 柠

//ning2   C4FC 狞

//ning2   C4FD 凝

//ning2   C4FE 宁*

//ning2   C5A1 拧*

//ning2   DFCC 咛

//ning2   E5B8 甯

//ning2   F1F7 聍

//ning3   C5A1 拧*

//ning4   C4FE 宁*

//ning4   C5A1 拧*

//ning4   C5A2 泞

//ning4   D8FA 佞

//niu1    E6A4 妞

//niu2    C5A3 牛

//niu3    C5A4 扭

//niu3    C5A5 钮

//niu3    C5A6 纽

//niu3    E1F0 狃

//niu3    E2EE 忸

//niu4    DED6 拗*

//nong2   C5A7 脓

//nong2   C5A8 浓

//nong2   C5A9 农

//nong2   D9AF 侬

//nong2   DFE6 哝

//nong4   C5AA 弄*

//nou4    F1F1 耨

//nu2     C5AB 奴

//nu2     E6DB 孥

//nu2     E6E5 驽

//nu3     C5AC 努

//nu3     E5F3 弩

//nu3     E6C0 胬

//nu4     C5AD 怒

//nu:3    C5AE 女

//nu:3    EECF 钕

//nu:4    EDA4 恧

//nu:4    F4AC 衄

//nuan3   C5AF 暖

//nue:4   C5B0 虐

//nue:4   C5B1 疟*

//nuo2    C4C8 娜*

//nuo2    C5B2 挪

//nuo2    D9D0 傩

//nuo4    C5B3 懦

//nuo4    C5B4 糯

//nuo4    C5B5 诺

//nuo4    DEF9 搦

//nuo4    DFF6 喏*

//nuo4    EFBB 锘

//o1      E0B8 喔*

//o1      E0DE 噢

//o2      C5B6 哦*

//o4      C5B6 哦*

//ou1     C5B7 欧

//ou1     C5B8 鸥

//ou1     C5B9 殴

//ou1     C5BD 沤*

//ou1     C7F8 区*

//ou1     DAA9 讴

//ou1     EAB1 瓯

//ou3     C5BA 藕

//ou3     C5BB 呕

//ou3     C5BC 偶

//ou3     F1EE 耦

//ou4     C5BD 沤*

//ou4     E2E6 怄

//pa1     C5BE 啪

//pa1     C5BF 趴

//pa1     C5C9 派*

//pa1     DDE2 葩

//pa2     B0C7 扒*

//pa2     B0D2 耙*

//pa2     C5C0 爬

//pa2     C5C3 琶

//pa2     E8CB 杷

//pa2     EED9 钯*

//pa2     F3E1 筢

//pa4     C5C1 帕

//pa4     C5C2 怕

//pai1    C5C4 拍

//pai2    C5C5 排*

//pai2    C5C6 牌

//pai2    C5C7 徘

//pai2    D9BD 俳

//pai3    C5C5 排*

//pai3    C6C8 迫*

//pai4    C5C8 湃

//pai4    C5C9 派*

//pai4    DDE5 蒎

//pai4    DFDF 哌

//pan1    B7AC 番*

//pan1    C5CA 攀

//pan1    C5CB 潘

//pan2    C5CC 盘

//pan2    C5CD 磐

//pan2    C5D6 胖*

//pan2    E3DD 爿

//pan2    F3B4 蟠

//pan2    F5E7 蹒

//pan4    C5CE 盼

//pan4    C5CF 畔

//pan4    C5D0 判

//pan4    C5D1 叛

//pan4    DED5 拚*

//pan4    E3FA 泮

//pan4    F1C8 袢

//pan4    F1E1 襻

//pang1   B0F2 膀*

//pang1   C5D2 乓

//pang1   E4E8 滂

//pang2   B0F2 膀*

//pang2   B0F5 磅*

//pang2   C5D3 庞

//pang2   C5D4 旁

//pang2   E1DD 彷*

//pang2   E5CC 逄

//pang2   F3A6 螃

//pang3   C5D5 耪

//pang4   C5D6 胖*

//pao1    C5D7 抛

//pao1    C5DD 泡*

//pao1    EBE3 脬

//pao2    C5D8 咆

//pao2    C5D9 刨*

//pao2    C5DA 炮*

//pao2    C5DB 袍

//pao2    C5DC 跑*

//pao2    DECB 匏

//pao2    E1F3 狍

//pao2    E2D2 庖

//pao3    C5DC 跑*

//pao4    C5DA 炮*

//pao4    C5DD 泡*

//pao4    F0E5 疱

//pei1    C5DE 呸

//pei1    C5DF 胚

//pei1    F5AC 醅

//pei2    C5E0 培

//pei2    C5E1 裴

//pei2    C5E2 赔

//pei2    C5E3 陪

//pei2    EFC2 锫

//pei4    C5E4 配

//pei4    C5E5 佩

//pei4    C5E6 沛

//pei4    E0CE 辔

//pei4    E0FA 帔

//pei4    ECB7 旆

//pei4    F6AC 霈

//pen1    C5E7 喷*

//pen2    C5E8 盆

//pen2    E4D4 湓

//pen4    C5E7 喷*

//peng1   C5E9 砰

//peng1   C5EA 抨

//peng1   C5EB 烹

//peng1   C5EC 澎*

//peng1   E0D8 嘭

//peng1   E2F1 怦

//peng2   C5EC 澎*

//peng2   C5ED 彭

//peng2   C5EE 蓬

//peng2   C5EF 棚

//peng2   C5F0 硼

//peng2   C5F1 篷

//peng2   C5F2 膨

//peng2   C5F3 朋

//peng2   C5F4 鹏

//peng2   DCA1 堋

//peng2   F3B2 蟛

//peng3   C5F5 捧

//peng4   C5F6 碰

//pi1     BBB5 坏*

//pi1     C5F7 坯

//pi1     C5F8 砒

//pi1     C5F9 霹

//pi1     C5FA 批

//pi1     C5FB 披

//pi1     C5FC 劈*

//pi1     D8A7 丕

//pi1     DAFC 邳

//pi1     E0E8 噼

//pi1     E7A2 纰

//pi1     EEEB 铍

//pi2     C5FD 琵

//pi2     C5FE 毗

//pi2     C6A1 啤

//pi2     C6A2 脾

//pi2     C6A3 疲

//pi2     C6A4 皮

//pi2     DAE9 陂*

//pi2     DAF0 陴

//pi2     DBAF 郫

//pi2     DBFD 埤*

//pi2     DCB1 鼙

//pi2     DCC5 芘

//pi2     E8C1 枇

//pi2     EEBC 罴

//pi2     F1D4 裨*

//pi2     F2B7 蚍

//pi2     F2E7 蜱

//pi2     F5F9 貔

//pi3     B7F1 否*

//pi3     C5FC 劈*

//pi3     C6A5 匹

//pi3     C6A6 痞

//pi3     D8F2 仳

//pi3     DBDC 圮

//pi3     DFA8 擗

//pi3     DFC1 吡*

//pi3     E2CF 庀

//pi3     F1B1 癖

//pi3     F1E2 疋*

//pi4     B1D9 辟*

//pi4     C6A7 僻

//pi4     C6A8 屁

//pi4     C6A9 譬

//pi4     DBFD 埤*

//pi4     E4C4 淠

//pi4     E6C7 媲

//pi4     EAB6 甓

//pi4     EEA2 睥*

//pian1   B1E2 扁*

//pian1   C6AA 篇

//pian1   C6AB 偏

//pian1   C6AC 片*

//pian1   EAFA 犏

//pian1   F4E6 翩

//pian2   B1E3 便*

//pian2   E6E9 骈

//pian2   E7C2 缏*

//pian2   EBDD 胼

//pian2   F5E4 蹁

//pian3   DAD2 谝

//pian4   C6AC 片*

//pian4   C6AD 骗

//piao1   C6AE 飘

//piao1   C6AF 漂*

//piao1   D8E2 剽

//piao1   E7CE 缥*

//piao1   F3AA 螵

//piao2   C6B0 瓢

//piao2   C6D3 朴*

//piao2   E6CE 嫖

//piao3   C6AF 漂*

//piao3   DDB3 莩*

//piao3   E7CE 缥*

//piao3   E9E8 殍

//piao3   EEA9 瞟

//piao4   C6AF 漂*

//piao4   C6B1 票

//piao4   E0D1 嘌

//piao4   E6F4 骠*

//pie1    C6B2 撇*

//pie1    C6B3 瞥

//pie1    EBAD 氕

//pie3    C6B2 撇*

//pie3    DCD6 苤

//pin1    C6B4 拼

//pin1    DED5 拚*

//pin1    E6B0 姘

//pin2    C6B5 频

//pin2    C6B6 贫

//pin2    E6C9 嫔

//pin2    F2AD 颦

//pin3    C6B7 品

//pin3    E9AF 榀

//pin4    C6B8 聘

//pin4    EAF2 牝

//ping1   C6B9 乒

//ping1   D9B7 俜

//ping1   E6B3 娉

//ping2   C6BA 坪

//ping2   C6BB 苹

//ping2   C6BC 萍

//ping2   C6BD 平

//ping2   C6BE 凭

//ping2   C6BF 瓶

//ping2   C6C0 评

//ping2   C6C1 屏*

//ping2   E8D2 枰

//ping2   F6D2 鲆

//po1     B2B4 泊*

//po1     C6C2 坡

//po1     C6C3 泼

//po1     C6C4 颇

//po1     C6D3 朴*

//po1     DAE9 陂*

//po1     E3F8 泺*

//po1     EAB7 攴

//po1     EEC7 钋

//po2     B7B1 繁*

//po2     C6C5 婆

//po2     DBB6 鄱

//po2     F0AB 皤

//po3     D8CF 叵

//po3     EEDE 钷

//po3     F3CD 笸

//po4     C6C6 破

//po4     C6C7 魄*

//po4     C6C8 迫*

//po4     C6C9 粕

//po4     C6D3 朴*

//po4     E7EA 珀

//pou1    C6CA 剖

//pou2    D9F6 裒

//pou2    DEE5 掊*

//pou3    DEE5 掊*

//pu1     C6CB 扑

//pu1     C6CC 铺*

//pu1     C6CD 仆*

//pu1     E0DB 噗

//pu2     B8AC 脯*

//pu2     C6CD 仆*

//pu2     C6CE 莆

//pu2     C6CF 葡

//pu2     C6D0 菩

//pu2     C6D1 蒲

//pu2     D9E9 匍

//pu2     E5A7 濮

//pu2     E8B1 璞

//pu2     EFE4 镤

//pu3     C6D2 埔*

//pu3     C6D3 朴*

//pu3     C6D4 圃

//pu3     C6D5 普

//pu3     C6D6 浦

//pu3     C6D7 谱

//pu3     E4DF 溥

//pu3     EBAB 氆

//pu3     EFE8 镨

//pu3     F5EB 蹼

//pu4     B1A4 堡*

//pu4     B1A9 暴*

//pu4     C6CC 铺*

//pu4     C6D8 曝*

//pu4     C6D9 瀑*

//qi1     BCA9 缉*

//qi1     C6DA 期*

//qi1     C6DB 欺

//qi1     C6DC 栖*

//qi1     C6DD 戚

//qi1     C6DE 妻*

//qi1     C6DF 七

//qi1     C6E0 凄

//qi1     C6E1 漆*

//qi1     C6E2 柒

//qi1     C6E3 沏

//qi1     DDC2 萋

//qi1     E0D2 嘁

//qi1     E8E7 桤

//qi1     ECA5 欹*

//qi1     F5E8 蹊*

//qi2     C6E4 其*

//qi2     C6E5 棋

//qi2     C6E6 奇*

//qi2     C6E7 歧

//qi2     C6E8 畦

//qi2     C6E9 崎

//qi2     C6EA 脐

//qi2     C6EB 齐*

//qi2     C6EC 旗

//qi2     C6ED 祈

//qi2     C6EE 祁

//qi2     C6EF 骑

//qi2     D8C1 亓

//qi2     D9B9 俟*

//qi2     DBDF 圻

//qi2     DCCE 芪

//qi2     DDBD 萁

//qi2     DEAD 蕲

//qi2     E1AA 岐

//qi2     E4BF 淇

//qi2     E6EB 骐

//qi2     E7F7 琪

//qi2     E7F9 琦

//qi2     EAC8 耆

//qi2     ECF7 祺

//qi2     F1FD 颀

//qi2     F2D3 蛴

//qi2     F2E0 蜞

//qi2     F4EB 綦

//qi2     F7A2 鳍

//qi2     F7E8 麒

//qi3     BBFC 稽*

//qi3     C6F0 起

//qi3     C6F1 岂

//qi3     C6F2 乞

//qi3     C6F3 企

//qi3     C6F4 启

//qi3     DCBB 芑

//qi3     E1A8 屺

//qi3     E7B2 绮

//qi3     E8BD 杞

//qi3     F4EC 綮*

//qi4     C6DE 妻*

//qi4     C6F5 契

//qi4     C6F6 砌

//qi4     C6F7 器

//qi4     C6F8 气

//qi4     C6F9 迄

//qi4     C6FA 弃

//qi4     C6FB 汽

//qi4     C6FC 泣*

//qi4     C6FD 讫

//qi4     D8BD 亟*

//qi4     DDDD 葺

//qi4     E3E0 汔

//qi4     E9CA 槭

//qi4     EDAC 憩

//qi4     EDD3 碛

//qi5     DCF9 荠*

//qia1    C6FE 掐

//qia1    DDD6 葜

//qia1    F1CA 袷*

//qia3    BFA8 卡*

//qia4    C7A1 恰

//qia4    C7A2 洽

//qia4    F7C4 髂

//qian1   C7A3 牵

//qian1   C7A4 扦

//qian1   C7A5 钎

//qian1   C7A6 铅*

//qian1   C7A7 千

//qian1   C7A8 迁

//qian1   C7A9 签

//qian1   C7AA 仟

//qian1   C7AB 谦

//qian1   D9DD 佥

//qian1   DAE4 阡

//qian1   DCB7 芊

//qian1   E1A9 岍

//qian1   E3A5 悭

//qian1   E5B9 骞

//qian1   E5BA 搴

//qian1   E5BD 褰

//qian1   EDA9 愆

//qian2   C7AC 乾*

//qian2   C7AD 黔

//qian2   C7AE 钱

//qian2   C7AF 钳

//qian2   C7B0 前

//qian2   C7B1 潜

//qian2   DDA1 荨*

//qian2   DEE7 掮

//qian2   EAF9 犍*

//qian2   EED4 钤

//qian2   F2AF 虔

//qian2   F3E9 箝

//qian3   C7B2 遣

//qian3   C7B3 浅*

//qian3   C7B4 谴

//qian3   E7D7 缱

//qian3   EBC9 肷

//qian4   C7B5 堑

//qian4   C7B6 嵌*

//qian4   C7B7 欠

//qian4   C7B8 歉

//qian4   CFCB 纤*

//qian4   D9BB 倩

//qian4   DCCD 芡

//qian4   DCE7 茜*

//qian4   E3BB 慊*

//qian4   E8FD 椠

//qiang1  C7B9 枪

//qiang1  C7BA 呛*

//qiang1  C7BB 腔

//qiang1  C7BC 羌

//qiang1  C7C0 抢*

//qiang1  E3DE 戕

//qiang1  EAA8 戗*

//qiang1  EFBA 锖

//qiang1  EFCF 锵

//qiang1  EFEA 镪*

//qiang1  F2DE 蜣

//qiang1  F5C4 跄*

//qiang2  C7BD 墙

//qiang2  C7BE 蔷

//qiang2  C7BF 强*

//qiang2  E6CD 嫱

//qiang2  E9C9 樯

//qiang3  C7C0 抢*

//qiang3  EFEA 镪*

//qiang3  F1DF 襁

//qiang3  F4C7 羟

//qiang4  C7BA 呛*

//qiang4  EAA8 戗*

//qiang4  ECC1 炝

//qiang4  F5C4 跄*

//qiao1   C7C1 橇

//qiao1   C7C2 锹

//qiao1   C7C3 敲

//qiao1   C7C4 悄*

//qiao1   C8B8 雀*

//qiao1   D8E4 劁

//qiao1   E7D8 缲*

//qiao1   EDCD 硗

//qiao1   F5CE 跷

//qiao2   C7C5 桥

//qiao2   C7C6 瞧

//qiao2   C7C7 乔

//qiao2   C7C8 侨

//qiao2   C7CC 翘*

//qiao2   DADB 谯

//qiao2   DCF1 荞

//qiao2   E1BD 峤*

//qiao2   E3BE 憔

//qiao2   E9D4 樵

//qiao2   F7B3 鞒

//qiao3   C7C4 悄*

//qiao3   C7C9 巧

//qiao3   C8B8 雀*

//qiao3   E3B8 愀

//qiao4   BFC7 壳*

//qiao4   C7CA 鞘*

//qiao4   C7CB 撬

//qiao4   C7CC 翘*

//qiao4   C7CD 峭

//qiao4   C7CE 俏

//qiao4   C7CF 窍

//qiao4   DABD 诮

//qie1    C7D0 切*

//qie2    C7D1 茄*

//qie3    C7D2 且*

//qie4    C7D0 切*

//qie4    C7D3 怯

//qie4    C7D4 窃

//qie4    DBA7 郄

//qie4    E3AB 惬

//qie4    E3BB 慊*

//qie4    E6AA 妾

//qie4    EAFC 挈

//qie4    EFC6 锲

//qie4    F3E6 箧

//qie4    F4F2 趄*

//qin1    C7D5 钦

//qin1    C7D6 侵

//qin1    C7D7 亲*

//qin1    F4C0 衾

//qin2    C7D8 秦

//qin2    C7D9 琴

//qin2    C7DA 勤

//qin2    C7DB 芹

//qin2    C7DC 擒

//qin2    C7DD 禽

//qin2    DCCB 芩

//qin2    E0BA 嗪

//qin2    E0DF 噙

//qin2    E2DB 廑*

//qin2    E4DA 溱*

//qin2    E9D5 檎

//qin2    F1E6 矜*

//qin2    F1FB 覃*

//qin2    F2FB 螓

//qin3    C7DE 寝

//qin3    EFB7 锓

//qin4    C7DF 沁

//qin4    DEEC 揿

//qin4    DFC4 吣

//qing1   C7E0 青

//qing1   C7E1 轻

//qing1   C7E2 氢

//qing1   C7E3 倾

//qing1   C7E4 卿

//qing1   C7E5 清

//qing1   E0F5 圊

//qing1   F2DF 蜻

//qing1   F6EB 鲭

//qing2   C7E6 擎

//qing2   C7E7 晴

//qing2   C7E8 氰

//qing2   C7E9 情

//qing2   E9D1 檠

//qing2   F7F4 黥

//qing3   C7EA 顷

//qing3   C7EB 请

//qing3   DCDC 苘

//qing3   F4EC 綮*

//qing3   F6A5 謦

//qing4   C7D7 亲*

//qing4   C7EC 庆

//qing4   EDE0 磬

//qing4   F3C0 罄

//qing4   F3E4 箐

//qiong2  C7ED 琼

//qiong2  C7EE 穷

//qiong2  DAF6 邛

//qiong2  DCE4 茕

//qiong2  F1B7 穹

//qiong2  F2CB 蛩

//qiong2  F3CC 筇

//qiong2  F5BC 跫

//qiong2  F6C6 銎

//qiu1    B9EA 龟*

//qiu1    C7EF 秋

//qiu1    C7F0 丘

//qiu1    C7F1 邱

//qiu1    E4D0 湫*

//qiu1    E9B1 楸

//qiu1    F2C7 蚯

//qiu1    F6FA 鳅

//qiu2    B3F0 仇*

//qiu2    C7F2 球

//qiu2    C7F3 求

//qiu2    C7F4 囚

//qiu2    C7F5 酋

//qiu2    C7F6 泅

//qiu2    D9B4 俅

//qiu2    DBCF 巯

//qiu2    E1EC 犰

//qiu2    E5CF 逑

//qiu2    E5D9 遒

//qiu2    EAE4 赇

//qiu2    F2B0 虬

//qiu2    F2F8 蝤*

//qiu2    F4C3 裘

//qiu2    F7FC 鼽

//qiu3    F4DC 糗

//qu1     C7F7 趋

//qu1     C7F8 区*

//qu1     C7F9 蛆

//qu1     C7FA 曲*

//qu1     C7FB 躯

//qu1     C7FC 屈

//qu1     C7FD 驱

//qu1     DAB0 诎

//qu1     E1AB 岖

//qu1     EAEF 觑*

//qu1     ECEE 祛

//qu1     F2D0 蛐

//qu1     F4F0 麴

//qu1     F7F1 黢

//qu2     C7FE 渠

//qu2     DBBE 劬

//qu2     DEA1 蕖

//qu2     DEBE 蘧

//qu2     E1E9 衢

//qu2     E8B3 璩

//qu2     EBAC 氍

//qu2     EBD4 朐

//qu2     EDE1 磲

//qu2     F0B6 鸲

//qu2     F1B3 癯

//qu2     F3BD 蠼

//qu2     F6C4 瞿*

//qu3     C7FA 曲*

//qu3     C8A1 取

//qu3     C8A2 娶

//qu3     C8A3 龋

//qu3     DCC4 苣*

//qu4     C8A4 趣

//qu4     C8A5 去

//qu4     E3D6 阒

//qu4     EAEF 觑*

//qu5     D0E7 戌*

//quan1   C8A6 圈*

//quan1   E3AA 悛

//quan2   C8A7 颧

//quan2   C8A8 权

//quan2   C8A9 醛

//quan2   C8AA 泉

//quan2   C8AB 全

//quan2   C8AC 痊

//quan2   C8AD 拳

//quan2   DAB9 诠

//quan2   DCF5 荃

//quan2   E9FA 辁

//quan2   EEFD 铨

//quan2   F2E9 蜷

//quan2   F3DC 筌

//quan2   F7DC 鬈

//quan3   C8AE 犬

//quan3   E7B9 绻

//quan3   EEB0 畎

//quan4   C8AF 券*

//quan4   C8B0 劝

//que1    C8B1 缺

//que1    C8B2 炔*

//que1    E3DA 阙*

//que2    C8B3 瘸

//que4    C8B4 却

//que4    C8B5 鹊

//que4    C8B6 榷

//que4    C8B7 确

//que4    C8B8 雀*

//que4    E3D7 阕

//que4    E3DA 阙*

//que4    EDA8 悫

//qun1    E5D2 逡

//qun2    C8B9 裙

//qun2    C8BA 群

//qun2    F7E5 麇*

//ran2    C8BB 然

//ran2    C8BC 燃

//ran2    F2C5 蚺

//ran2    F7D7 髯

//ran3    C8BD 冉

//ran3    C8BE 染

//ran3    DCDB 苒

//rang1   C8C2 嚷*

//rang2   C8BF 瓤

//rang2   ECFC 禳

//rang2   F0A6 穰

//rang3   C8C0 壤

//rang3   C8C1 攘

//rang3   C8C2 嚷*

//rang4   C8C3 让

//rao2    C8C4 饶

//rao2    DCE9 荛

//rao2    E6AC 娆*

//rao2    E8E3 桡

//rao3    C8C5 扰

//rao3    E6AC 娆*

//rao5    C8C6 绕

//re3     C8C7 惹

//re3     C8F4 若*

//re3     DFF6 喏*

//re4     C8C8 热

//ren2    C8C9 壬

//ren2    C8CA 仁

//ren2    C8CB 人

//ren2    C8CE 任*

//ren3    C8CC 忍

//ren3    DCF3 荏

//ren3    EFFE 稔

//ren4    C8CD 韧

//ren4    C8CE 任*

//ren4    C8CF 认

//ren4    C8D0 刃

//ren4    C8D1 妊

//ren4    C8D2 纫

//ren4    D8F0 仞

//ren4    DDD8 葚*

//ren4    E2BF 饪

//ren4    E9ED 轫

//ren4    F1C5 衽

//reng1   C8D3 扔

//reng2   C8D4 仍

//ri4     C8D5 日

//rong2   C8D6 戎

//rong2   C8D7 茸

//rong2   C8D8 蓉

//rong2   C8D9 荣

//rong2   C8DA 融

//rong2   C8DB 熔

//rong2   C8DC 溶

//rong2   C8DD 容

//rong2   C8DE 绒

//rong2   E1C9 嵘

//rong2   E1F5 狨

//rong2   E9C5 榕

//rong2   EBC0 肜

//rong2   F2EE 蝾

//rong3   C8DF 冗

//rou2    C8E0 揉

//rou2    C8E1 柔

//rou2    F4DB 糅

//rou2    F5E5 蹂

//rou2    F7B7 鞣

//rou4    C8E2 肉

//ru2     C8E3 茹

//ru2     C8E4 蠕

//ru2     C8E5 儒

//ru2     C8E6 孺

//ru2     C8E7 如

//ru2     DEB8 薷

//ru2     E0E9 嚅

//ru2     E5A6 濡

//ru2     EFA8 铷

//ru2     F1E0 襦

//ru2     F2AC 颥

//ru3     C8E8 辱

//ru3     C8E9 乳

//ru3     C8EA 汝

//ru4     C8EB 入

//ru4     C8EC 褥

//ru4     DDEA 蓐

//ru4     E4B2 洳

//ru4     E4E1 溽

//ru4     E7C8 缛

//ruan3   C8ED 软

//ruan3   C8EE 阮

//ruan3   EBC3 朊

//rui2    DEA8 蕤

//rui3    C8EF 蕊

//rui4    C8F0 瑞

//rui4    C8F1 锐

//rui4    DCC7 芮

//rui4    E8C4 枘

//rui4    EEA3 睿

//rui4    F2B8 蚋

//run4    C8F2 闰

//run4    C8F3 润

//ruo4    C8F4 若*

//ruo4    C8F5 弱

//ruo4    D9BC 偌

//ruo4    F3E8 箬

//sa1     C8F6 撒*

//sa1     CBBC 思*

//sa1     D8ED 仨

//sa1     EAFD 挲*

//sa3     C8F6 撒*

//sa3     C8F7 洒

//sa4     C8F8 萨

//sa4     D8A6 卅

//sa4     EBDB 脎

//sa4     ECAA 飒

//sai1    C8F9 腮

//sai1    C8FA 鳃

//sai1    C8FB 塞*

//sai1    CBBC 思*

//sai1    E0E7 噻

//sai4    C8FB 塞*

//sai4    C8FC 赛

//san1    C8FD 三

//san1    C8FE 叁

//san1    EBA7 毵

//san3    C9A1 伞

//san3    C9A2 散*

//san3    E2CC 馓

//san3    F4D6 糁*

//san4    C9A2 散*

//sang1   C9A3 桑

//sang1   C9A5 丧*

//sang3   C9A4 嗓

//sang3   DEFA 搡

//sang3   EDDF 磉

//sang3   F2AA 颡

//sang4   C9A5 丧*

//sao1    C9A6 搔

//sao1    C9A7 骚

//sao1    E7D2 缫

//sao1    E7D8 缲*

//sao1    EBFD 臊*

//sao1    F6FE 鳋

//sao3    C9A8 扫*

//sao3    C9A9 嫂

//sao4    C9A8 扫*

//sao4    C9D2 梢*

//sao4    DCA3 埽

//sao4    EBFD 臊*

//sao4    F0FE 瘙

//se4     C8FB 塞*

//se4     C9AA 瑟

//se4     C9AB 色*

//se4     C9AC 涩

//se4     D8C4 啬

//se4     EFA4 铯

//se4     F0A3 穑

//sen1    C9AD 森

//seng1   C9AE 僧

//sha1    C9AF 莎*

//sha1    C9B0 砂

//sha1    C9B1 杀

//sha1    C9B2 刹*

//sha1    C9B3 沙*

//sha1    C9B4 纱

//sha1    C9B7 煞*

//sha1    C9BC 杉*

//sha1    EAFD 挲*

//sha1    EFA1 铩

//sha1    F0F0 痧

//sha1    F4C4 裟

//sha1    F6E8 鲨

//sha2    C9B6 啥

//sha3    C9B5 傻

//sha4    C9B3 沙*

//sha4    C9B7 煞*

//sha4    CFC3 厦*

//sha4    DFFE 唼

//sha4    E0C4 嗄*

//sha4    ECA6 歃

//sha4    F6AE 霎

//shai1   C9B8 筛

//shai1   F5A7 酾*

//shai3   C9AB 色*

//shai4   C9B9 晒

//shan1   C9BA 珊

//shan1   C9BB 苫*

//shan1   C9BC 杉*

//shan1   C9BD 山

//shan1   C9BE 删

//shan1   C9BF 煽

//shan1   C9C0 衫

//shan1   C9C8 扇*

//shan1   D5A4 栅*

//shan1   DBEF 埏

//shan1   DCCF 芟

//shan1   E4FA 潸

//shan1   E6A9 姗

//shan1   EBFE 膻

//shan1   EECC 钐*

//shan1   F4AE 舢

//shan1   F5C7 跚

//shan3   B2F4 掺*

//shan3   C9C1 闪

//shan3   C9C2 陕

//shan4   B5A5 单*

//shan4   B5A7 掸*

//shan4   C9BB 苫*

//shan4   C9C3 擅

//shan4   C9C4 赡

//shan4   C9C5 膳

//shan4   C9C6 善

//shan4   C9C7 汕

//shan4   C9C8 扇*

//shan4   C9C9 缮

//shan4   D8DF 剡*

//shan4   DAA8 讪

//shan4   DBB7 鄯

//shan4   E6D3 嬗

//shan4   E6F3 骟

//shan4   ECF8 禅*

//shan4   EECC 钐*

//shan4   F0DE 疝

//shan4   F3B5 蟮

//shan4   F7AD 鳝

//shang1  C9CA 墒

//shang1  C9CB 伤

//shang1  C9CC 商

//shang1  CCC0 汤*

//shang1  E9E4 殇

//shang1  ECD8 熵

//shang1  F5FC 觞

//shang3  C9CD 赏

//shang3  C9CE 晌

//shang3  C9CF 上*

//shang3  DBF0 垧

//shang4  C9CF 上*

//shang4  C9D0 尚

//shang4  E7B4 绱

//shang5  C9D1 裳*

//shao1   C7CA 鞘*

//shao1   C9D2 梢*

//shao1   C9D3 捎

//shao1   C9D4 稍*

//shao1   C9D5 烧

//shao1   F2D9 蛸*

//shao1   F3E2 筲

//shao1   F4B9 艄

//shao2   C9D6 芍

//shao2   C9D7 勺

//shao2   C9D8 韶

//shao2   DCE6 苕*

//shao2   E8BC 杓*

//shao3   C9D9 少*

//shao4   C9D4 稍*

//shao4   C9D9 少*

//shao4   C9DA 哨

//shao4   C9DB 邵

//shao4   C9DC 绍

//shao4   D5D9 召*

//shao4   DBBF 劭

//shao4   E4FB 潲

//she1    C9DD 奢

//she1    C9DE 赊

//she1    E2A6 猞

//she1    EEB4 畲

//she2    C9DF 蛇*

//she2    C9E0 舌

//she2    D5DB 折*

//she2    D9DC 佘

//she2    DEE9 揲*

//she3    C9E1 舍*

//she4    C9E1 舍*

//she4    C9E2 赦

//she4    C9E3 摄

//she4    C9E4 射

//she4    C9E5 慑

//she4    C9E6 涉

//she4    C9E7 社

//she4    C9E8 设

//she4    D8C7 厍

//she4    E4DC 滠

//she4    ECA8 歙*

//she4    F7EA 麝

//shei2   CBAD 谁*

//shen1   B2CE 参*

//shen1   C9E9 砷

//shen1   C9EA 申

//shen1   C9EB 呻

//shen1   C9EC 伸

//shen1   C9ED 身

//shen1   C9EE 深

//shen1   C9EF 娠

//shen1   C9F0 绅

//shen1   DAB7 诜

//shen1   DDB7 莘*

//shen1   F4D6 糁*

//shen2   C9F1 神

//shen2   C9F5 甚*

//shen2   CAB2 什*

//shen3   C9F2 沈*

//shen3   C9F3 审

//shen3   C9F4 婶

//shen3   DAC5 谂

//shen3   DFD3 哂

//shen3   E4C9 渖

//shen3   EFF2 矧

//shen4   C9F5 甚*

//shen4   C9F6 肾

//shen4   C9F7 慎

//shen4   C9F8 渗

//shen4   DDD8 葚*

//shen4   E9A9 椹*

//shen4   EBCF 胂

//shen4   F2D7 蜃

//sheng1  C9F9 声

//sheng1  C9FA 生

//sheng1  C9FB 甥

//sheng1  C9FC 牲

//sheng1  C9FD 升

//sheng1  CAA4 胜*

//sheng1  F3CF 笙

//sheng2  C9FE 绳

//sheng2  E4C5 渑*

//sheng3  CAA1 省*

//sheng3  EDF2 眚

//sheng4  B3CB 乘*

//sheng4  CAA2 盛*

//sheng4  CAA3 剩

//sheng4  CAA4 胜*

//sheng4  CAA5 圣

//sheng4  E1D3 嵊

//sheng4  EAC9 晟

//shi1    CAA6 师

//shi1    CAA7 失

//shi1    CAA8 狮

//shi1    CAA9 施

//shi1    CAAA 湿

//shi1    CAAB 诗

//shi1    CAAC 尸

//shi1    CAAD 虱

//shi1    D0EA 嘘*

//shi1    DDE9 蓍

//shi1    F5A7 酾*

//shi1    F6F5 鲺

//shi2    CAAE 十

//shi2    CAAF 石*

//shi2    CAB0 拾

//shi2    CAB1 时

//shi2    CAB2 什*

//shi2    CAB3 食*

//shi2    CAB4 蚀

//shi2    CAB5 实

//shi2    CAB6 识*

//shi2    DBF5 埘

//shi2    DDAA 莳*

//shi2    ECC2 炻

//shi2    F6E5 鲥

//shi3    CAB7 史

//shi3    CAB8 矢

//shi3    CAB9 使

//shi3    CABA 屎

//shi3    CABB 驶

//shi3    CABC 始

//shi3    F5B9 豕

//shi4    CABD 式

//shi4    CABE 示

//shi4    CABF 士

//shi4    CAC0 世

//shi4    CAC1 柿

//shi4    CAC2 事

//shi4    CAC3 拭

//shi4    CAC4 誓

//shi4    CAC5 逝

//shi4    CAC6 势

//shi4    CAC7 是

//shi4    CAC8 嗜

//shi4    CAC9 噬

//shi4    CACA 适*

//shi4    CACB 仕

//shi4    CACC 侍

//shi4    CACD 释

//shi4    CACE 饰

//shi4    CACF 氏*

//shi4    CAD0 市

//shi4    CAD1 恃

//shi4    CAD2 室

//shi4    CAD3 视

//shi4    CAD4 试

//shi4    CBC6 似*

//shi4    D6C5 峙*

//shi4    DAD6 谥

//shi4    DDAA 莳*

//shi4    DFB1 弑

//shi4    E9F8 轼

//shi4    EADB 贳

//shi4    EEE6 铈

//shi4    F3A7 螫*

//shi4    F3C2 舐

//shi4    F3DF 筮

//shi5    B3D7 匙*

//shi5    D6B3 殖*

//shou1   CAD5 收

//shou2   CAEC 熟*

//shou3   CAD6 手

//shou3   CAD7 首

//shou3   CAD8 守

//shou3   F4BC 艏

//shou4   CAD9 寿

//shou4   CADA 授

//shou4   CADB 售

//shou4   CADC 受

//shou4   CADD 瘦

//shou4   CADE 兽

//shou4   E1F7 狩

//shou4   E7B7 绶

//shu1    CADF 蔬

//shu1    CAE0 枢

//shu1    CAE1 梳

//shu1    CAE2 殊

//shu1    CAE3 抒

//shu1    CAE4 输

//shu1    CAE5 叔

//shu1    CAE6 舒

//shu1    CAE7 淑

//shu1    CAE8 疏

//shu1    CAE9 书

//shu1    D9BF 倏

//shu1    DDC4 菽

//shu1    DEF3 摅

//shu1    E6AD 姝

//shu1    E7A3 纾

//shu1    EBA8 毹

//shu1    ECAF 殳

//shu2    CAEA 赎

//shu2    CAEB 孰

//shu2    CAEC 熟*

//shu2    DBD3 塾

//shu2    EFF8 秫

//shu3    CAED 薯

//shu3    CAEE 暑

//shu3    CAEF 曙

//shu3    CAF0 署

//shu3    CAF1 蜀*

//shu3    CAF2 黍

//shu3    CAF3 鼠

//shu3    CAF4 属*

//shu3    CAFD 数*

//shu4    CAF5 术*

//shu4    CAF6 述

//shu4    CAF7 树

//shu4    CAF8 束

//shu4    CAF9 戍

//shu4    CAFA 竖

//shu4    CAFB 墅

//shu4    CAFC 庶

//shu4    CAFD 数*

//shu4    CAFE 漱

//shu4    CBA1 恕

//shu4    D3E1 俞*

//shu4    E3F0 沭

//shu4    E4F8 澍

//shu4    EBF2 腧

//shua1   CBA2 刷*

//shua1   E0A7 唰

//shua3   CBA3 耍

//shua4   CBA2 刷*

//shuai1  CBA4 摔

//shuai3  CBA6 甩

//shuai4  C2CA 率*

//shuai4  CBA7 帅

//shuai4  F3B0 蟀

//shuan1  CBA8 栓

//shuan1  CBA9 拴

//shuan1  E3C5 闩

//shuan4  E4CC 涮

//shuang1 CBAA 霜

//shuang1 CBAB 双

//shuang1 E3F1 泷*

//shuang1 E6D7 孀

//shuang3 CBAC 爽

//shui2   CBAD 谁*

//shui3   CBAE 水

//shui4   CBAF 睡

//shui4   CBB0 税

//shui4   CBB5 说*

//shun3   CBB1 吮

//shun4   CBB2 瞬

//shun4   CBB3 顺

//shun4   CBB4 舜

//shuo1   CBB5 说*

//shuo4   CAFD 数*

//shuo4   CBB6 硕

//shuo4   CBB7 朔

//shuo4   CBB8 烁

//shuo4   DDF4 蒴

//shuo4   DEF7 搠

//shuo4   E5F9 妁

//shuo4   E9C3 槊

//shuo4   EEE5 铄

//si1     CBB9 斯

//si1     CBBA 撕

//si1     CBBB 嘶

//si1     CBBD 私

//si1     CBBE 司

//si1     CBBF 丝

//si1     D8CB 厮

//si1     DBCC 厶

//si1     DFD0 咝

//si1     E4F9 澌

//si1     E7C1 缌

//si1     EFC8 锶

//si1     F0B8 鸶

//si1     F2CF 蛳

//si3     CBC0 死

//si4     CAB3 食*

//si4     CBC1 肆

//si4     CBC2 寺

//si4     CBC3 嗣

//si4     CBC4 四

//si4     CBC5 伺*

//si4     CBC6 似*

//si4     CBC7 饲

//si4     CBC8 巳

//si4     D9B9 俟*

//si4     D9EE 兕

//si4     E3E1 汜

//si4     E3F4 泗

//si4     E6A6 姒

//si4     E6E1 驷

//si4     ECEB 祀

//si4     F1EA 耜

//si4     F3D3 笥

//song1   CBC9 松

//song1   DAA1 凇

//song1   DDBF 菘

//song1   E1C2 崧

//song1   E1D4 嵩

//song1   E2EC 忪*

//song1   E4C1 淞

//song3   CBCA 耸

//song3   CBCB 怂

//song3   E3A4 悚

//song3   F1B5 竦

//song4   CBCC 颂

//song4   CBCD 送

//song4   CBCE 宋

//song4   CBCF 讼

//song4   CBD0 诵

//sou1    CBD1 搜

//sou1    CBD2 艘

//sou1    E0B2 嗖

//sou1    E2C8 馊

//sou1    E4D1 溲

//sou1    ECAC 飕

//sou1    EFCB 锼

//sou1    F2F4 螋

//sou3    CBD3 擞*

//sou3    DBC5 叟

//sou3    DEB4 薮

//sou3    E0D5 嗾

//sou3    EEA4 瞍

//sou4    CBD3 擞*

//sou4    CBD4 嗽

//su1     CBD5 苏

//su1     CBD6 酥

//su1     F6D5 稣

//su2     CBD7 俗

//su4     CBD8 素

//su4     CBD9 速

//su4     CBDA 粟

//su4     CBDB 僳

//su4     CBDC 塑

//su4     CBDD 溯

//su4     CBDE 宿*

//su4     CBDF 诉

//su4     CBE0 肃

//su4     CBF5 缩*

//su4     D9ED 夙

//su4     DAD5 谡

//su4     DDF8 蔌

//su4     E0BC 嗉

//su4     E3BA 愫

//su4     E4B3 涑

//su4     F3F9 簌

//su4     F6A2 觫

//suan1   CBE1 酸

//suan1   E2A1 狻

//suan4   CBE2 蒜

//suan4   CBE3 算

//sui1    C4F2 尿*

//sui1    CBE4 虽

//sui1    DDB4 荽

//sui1    E5A1 濉

//sui1    EDF5 眭

//sui1    EEA1 睢

//sui2    CBE5 隋

//sui2    CBE6 随

//sui2    CBE7 绥

//sui2    CBEC 遂*

//sui3    CBE8 髓

//sui4    CBE9 碎

//sui4    CBEA 岁

//sui4    CBEB 穗

//sui4    CBEC 遂*

//sui4    CBED 隧

//sui4    CBEE 祟

//sui4    DAC7 谇

//sui4    E5E4 邃

//sui4    ECDD 燧

//sun1    CBEF 孙

//sun1    DDA5 荪

//sun1    E1F8 狲

//sun1    E2B8 飧

//sun3    CBF0 损

//sun3    CBF1 笋

//sun3    E9BE 榫

//sun3    F6C0 隼

//suo1    C9AF 莎*

//suo1    CBF2 蓑

//suo1    CBF3 梭

//suo1    CBF4 唆

//suo1    CBF5 缩*

//suo1    E0C2 嗦

//suo1    E0CA 嗍

//suo1    E6B6 娑

//suo1    E8F8 桫

//suo1    EAFD 挲*

//suo1    EDFC 睃

//suo1    F4C8 羧

//suo3    CBF6 琐

//suo3    CBF7 索

//suo3    CBF8 锁

//suo3    CBF9 所

//suo3    DFEF 唢

//ta1     CBFA 塌

//ta1     CBFB 他

//ta1     CBFC 它

//ta1     CBFD 她

//ta1     CCA4 踏*

//ta1     E4E2 溻

//ta1     E5DD 遢

//ta1     EEE8 铊*

//ta1     F5C1 趿

//ta3     CBFE 塔

//ta3     CCA1 獭

//ta3     F7A3 鳎

//ta4     CCA2 挞

//ta4     CCA3 蹋

//ta4     CCA4 踏*

//ta4     CDD8 拓*

//ta4     E0AA 嗒*

//ta4     E3CB 闼

//ta4     E4F0 漯*

//ta4     E9BD 榻

//ta4     EDB3 沓*

//tai1    CCA5 胎

//tai1    CCA6 苔*

//tai1    CCA8 台*

//tai2    CCA6 苔*

//tai2    CCA7 抬

//tai2    CCA8 台*

//tai2    DBA2 邰

//tai2    DEB7 薹

//tai2    E6E6 骀*

//tai2    ECC6 炱

//tai2    F5CC 跆

//tai2    F6D8 鲐

//tai4    CCA9 泰

//tai4    CCAA 酞

//tai4    CCAB 太

//tai4    CCAC 态

//tai4    CCAD 汰

//tai4    EBC4 肽

//tai4    EED1 钛

//tan1    CCAE 坍

//tan1    CCAF 摊

//tan1    CCB0 贪

//tan1    CCB1 瘫

//tan1    CCB2 滩

//tan2    B5AF 弹*

//tan2    CCB3 坛

//tan2    CCB4 檀

//tan2    CCB5 痰

//tan2    CCB6 潭

//tan2    CCB7 谭

//tan2    CCB8 谈

//tan2    DBB0 郯

//tan2    E5A3 澹*

//tan2    EABC 昙

//tan2    EFC4 锬

//tan2    EFE2 镡*

//tan2    F1FB 覃*

//tan3    CCB9 坦

//tan3    CCBA 毯

//tan3    CCBB 袒

//tan3    ECFE 忐

//tan3    EEE3 钽

//tan4    CCBC 碳

//tan4    CCBD 探

//tan4    CCBE 叹

//tan4    CCBF 炭

//tang1   CCC0 汤*

//tang1   CCCB 趟*

//tang1   EFA6 铴

//tang1   EFDB 镗*

//tang1   F1ED 耥*

//tang1   F4CA 羰

//tang2   CCC1 塘

//tang2   CCC2 搪

//tang2   CCC3 堂

//tang2   CCC4 棠

//tang2   CCC5 膛

//tang2   CCC6 唐

//tang2   CCC7 糖

//tang2   E2BC 饧*

//tang2   E4E7 溏

//tang2   E8A9 瑭

//tang2   E9CC 樘

//tang2   EFDB 镗*

//tang2   F3A5 螗

//tang2   F3AB 螳

//tang2   F5B1 醣

//tang3   B3A8 敞*

//tang3   CCC9 躺

//tang3   CCCA 淌

//tang3   D8F6 伥*

//tang3   D9CE 傥

//tang3   E0FB 帑

//tang3   F1ED 耥*

//tang4   CCCB 趟*

//tang4   CCCC 烫

//tao1    CCCD 掏

//tao1    CCCE 涛

//tao1    CCCF 滔

//tao1    CCD0 绦

//tao1    DFB6 叨*

//tao1    E8BA 韬

//tao1    ECE2 焘*

//tao1    F7D2 饕

//tao2    CCD1 萄

//tao2    CCD2 桃

//tao2    CCD3 逃

//tao2    CCD4 淘

//tao2    CCD5 陶*

//tao2    D8BB 鼗

//tao2    DFFB 啕

//tao2    E4AC 洮

//tao3    CCD6 讨

//tao4    CCD7 套

//te4     CCD8 特

//te4     DFAF 忒*

//te4     ECFD 忑

//te4     EDAB 慝

//te4     EFAB 铽

//teng2   CCD9 藤

//teng2   CCDA 腾

//teng2   CCDB 疼

//teng2   CCDC 誊

//teng2   EBF8 滕

//ti1     CCDD 梯

//ti1     CCDE 剔

//ti1     CCDF 踢

//ti1     CCE0 锑

//ti1     CCE5 体*

//ti2     CCE1 提*

//ti2     CCE2 题

//ti2     CCE3 蹄

//ti2     CCE4 啼

//ti2     DCE8 荑*

//ti2     E7B0 绨*

//ti2     E7BE 缇

//ti2     F0C3 鹈

//ti2     F5AE 醍

//ti3     CCE5 体*

//ti4     CCE6 替

//ti4     CCE7 嚏

//ti4     CCE8 惕

//ti4     CCE9 涕

//ti4     CCEA 剃

//ti4     CCEB 屉

//ti4     D9C3 倜

//ti4     E3A9 悌

//ti4     E5D1 逖

//ti4     E7B0 绨*

//ti4     F1D3 裼*

//tian1   CCEC 天

//tian1   CCED 添

//tian2   B5E8 佃*

//tian2   CCEE 填

//tian2   CCEF 田

//tian2   CCF0 甜

//tian2   CCF1 恬

//tian2   E3D9 阗

//tian2   EEB1 畋

//tian2   EEE4 钿*

//tian3   CCF2 舔

//tian3   CCF3 腆

//tian3   E3C3 忝

//tian3   E9E5 殄

//tian4   DEDD 掭

//tiao1   CCF4 挑*

//tiao1   D9AC 佻

//tiao1   ECF6 祧

//tiao2   B5F7 调*

//tiao2   CCF5 条

//tiao2   CCF6 迢

//tiao2   DCE6 苕*

//tiao2   F2E8 蜩

//tiao2   F3D4 笤

//tiao2   F6B6 龆

//tiao2   F6E6 鲦

//tiao2   F7D8 髫

//tiao3   CCF4 挑*

//tiao3   F1BB 窕

//tiao4   CCF7 眺

//tiao4   CCF8 跳

//tiao4   F4D0 粜

//tie1    CCF9 贴

//tie1    CCFB 帖*

//tie1    DDC6 萜

//tie3    CCFA 铁

//tie3    CCFB 帖*

//tie4    CCFB 帖*

//tie4    F7D1 餮

//ting1   CCFC 厅

//ting1   CCFD 听

//ting1   CCFE 烃

//ting1   CDA1 汀

//ting2   CDA2 廷

//ting2   CDA3 停

//ting2   CDA4 亭

//ting2   CDA5 庭

//ting2   DCF0 莛

//ting2   DDE3 葶

//ting2   E6C3 婷

//ting2   F2D1 蜓

//ting2   F6AA 霆

//ting3   CDA6 挺

//ting3   CDA7 艇

//ting3   E8E8 梃*

//ting3   EEAE 町*

//ting3   EEFA 铤*

//ting4   E8E8 梃*

//tong1   CDA8 通*

//tong1   E0CC 嗵

//tong2   B6B1 侗*

//tong2   CDA9 桐

//tong2   CDAA 酮

//tong2   CDAB 瞳

//tong2   CDAC 同*

//tong2   CDAD 铜

//tong2   CDAE 彤

//tong2   CDAF 童

//tong2   D9A1 佟

//tong2   D9D7 僮*

//tong2   D9DA 仝

//tong2   DBED 垌*

//tong2   DCED 茼

//tong2   E1BC 峒*

//tong2   E4FC 潼

//tong2   EDC5 砼

//tong3   B6B1 侗*

//tong3   CDB0 桶

//tong3   CDB1 捅

//tong3   CDB2 筒

//tong3   CDB3 统

//tong4   CDA8 通*

//tong4   CDAC 同*

//tong4   CDB4 痛

//tong4   E2FA 恸

//tou1    CDB5 偷

//tou2    CDB6 投

//tou2    CDB7 头

//tou2    F7BB 骰

//tou3    EED7 钭

//tou4    CDB8 透

//tu1     CDB9 凸

//tu1     CDBA 秃

//tu1     CDBB 突

//tu2     CDBC 图

//tu2     CDBD 徒

//tu2     CDBE 途

//tu2     CDBF 涂

//tu2     CDC0 屠

//tu2     DDB1 荼

//tu2     DDCB 菟*

//tu2     F5A9 酴

//tu3     CDC1 土

//tu3     CDC2 吐*

//tu3     EECA 钍

//tu4     CDC2 吐*

//tu4     CDC3 兔

//tu4     DCA2 堍

//tu4     DDCB 菟*

//tuan1   CDC4 湍

//tuan2   CDC5 团

//tuan2   DED2 抟

//tuan3   EEB6 疃

//tuan4   E5E8 彖

//tui1    CDC6 推

//tui1    DFAF 忒*

//tui2    CDC7 颓

//tui3    CDC8 腿

//tui4    CDC9 蜕

//tui4    CDCA 褪*

//tui4    CDCB 退

//tui4    ECD5 煺

//tun1    CDCC 吞

//tun1    EAD5 暾

//tun2    B6DA 囤*

//tun2    CDCD 屯*

//tun2    CDCE 臀

//tun2    E2BD 饨

//tun2    EBE0 豚

//tun3    D9DB 氽

//tun4    CDCA 褪*

//tuo1    CDCF 拖

//tuo1    CDD0 托

//tuo1    CDD1 脱

//tuo1    D8B1 乇

//tuo2    CDD2 鸵

//tuo2    CDD3 陀

//tuo2    CDD4 驮*

//tuo2    CDD5 驼

//tuo2    D9A2 佗

//tuo2    DBE7 坨

//tuo2    E3FB 沱

//tuo2    E8DE 柁*

//tuo2    E9D2 橐

//tuo2    EDC8 砣

//tuo2    EEE8 铊*

//tuo2    F5A2 酡

//tuo2    F5C9 跎

//tuo2    F6BE 鼍

//tuo3    CDD6 椭

//tuo3    CDD7 妥

//tuo3    E2D5 庹

//tuo4    C6C7 魄*

//tuo4    CDD8 拓*

//tuo4    CDD9 唾

//tuo4    E8D8 柝

//tuo4    F3EA 箨

//wa1     CDDA 挖

//wa1     CDDB 哇*

//wa1     CDDC 蛙

//wa1     CDDD 洼

//wa1     E6B4 娲

//wa2     CDDE 娃

//wa3     CDDF 瓦*

//wa3     D8F4 佤

//wa4     CDDF 瓦*

//wa4     CDE0 袜

//wa4     EBF0 腽

//wa5     CDDB 哇*

//wai1    CDE1 歪

//wai3    E1CB 崴*

//wai4    CDE2 外

//wan1    CDE3 豌

//wan1    CDE4 弯

//wan1    CDE5 湾

//wan1    D8E0 剜

//wan1    F2EA 蜿

//wan2    CDE6 玩

//wan2    CDE7 顽

//wan2    CDE8 丸

//wan2    CDE9 烷

//wan2    CDEA 完

//wan2    DCB9 芄

//wan2    E6FD 纨

//wan3    CDEB 碗

//wan3    CDEC 挽

//wan3    CDED 晚

//wan3    CDEE 皖

//wan3    CDEF 惋

//wan3    CDF0 宛

//wan3    CDF1 婉

//wan3    DDB8 莞*

//wan3    DDD2 菀

//wan3    E7BA 绾

//wan3    E7FE 琬

//wan3    EBE4 脘

//wan3    EEB5 畹

//wan4    C2FB 蔓*

//wan4    CDF2 万*

//wan4    CDF3 腕

//wang1   CDF4 汪

//wang1   DECC 尢

//wang2   CDF5 王*

//wang2   CDF6 亡*

//wang3   CDF7 枉

//wang3   CDF8 网

//wang3   CDF9 往

//wang3   D8E8 罔

//wang3   E3AF 惘

//wang3   E9FE 辋

//wang3   F7CD 魍

//wang4   CDF5 王*

//wang4   CDFA 旺

//wang4   CDFB 望

//wang4   CDFC 忘

//wang4   CDFD 妄

//wei1    CDFE 威

//wei1    CEA1 巍

//wei1    CEA2 微

//wei1    CEA3 危

//wei1    CEAF 委*

//wei1    D9CB 偎

//wei1    DAF1 隈

//wei1    DDDA 葳

//wei1    DEB1 薇

//wei1    E1CB 崴*

//wei1    E5D4 逶

//wei1    ECD0 煨

//wei2    CEA4 韦

//wei2    CEA5 违

//wei2    CEA6 桅

//wei2    CEA7 围

//wei2    CEA8 唯

//wei2    CEA9 惟

//wei2    CEAA 为*

//wei2    CEAB 潍

//wei2    CEAC 维

//wei2    DBD7 圩*

//wei2    E0F8 帏

//wei2    E1A1 帷

//wei2    E1CD 嵬

//wei2    E3C7 闱

//wei2    E3ED 沩

//wei2    E4B6 涠

//wei3    CEAD 苇

//wei3    CEAE 萎

//wei3    CEAF 委*

//wei3    CEB0 伟

//wei3    CEB1 伪

//wei3    CEB2 尾*

//wei3    CEB3 纬

//wei3    DAC3 诿

//wei3    DAF3 隗*

//wei3    E2AB 猥

//wei3    E4A2 洧

//wei3    E6B8 娓

//wei3    E7E2 玮

//wei3    E8B8 韪

//wei3    ECBF 炜

//wei3    F0F4 痿

//wei3    F4BA 艉

//wei3    F6DB 鲔

//wei4    CEAA 为*

//wei4    CEB4 未

//wei4    CEB5 蔚*

//wei4    CEB6 味

//wei4    CEB7 畏

//wei4    CEB8 胃

//wei4    CEB9 喂

//wei4    CEBA 魏

//wei4    CEBB 位

//wei4    CEBC 渭

//wei4    CEBD 谓

//wei4    CEBE 尉*

//wei4    CEBF 慰

//wei4    CEC0 卫

//wei4    D2C5 遗*

//wei4    E2AC 猬

//wei4    EAA6 軎

//wen1    CEC1 瘟

//wen1    CEC2 温

//wen2    CEC3 蚊

//wen2    CEC4 文

//wen2    CEC5 闻

//wen2    CEC6 纹*

//wen2    E3D3 阌

//wen2    F6A9 雯

//wen3    CEC7 吻

//wen3    CEC8 稳

//wen3    CEC9 紊

//wen3    D8D8 刎

//wen4    CEC6 纹*

//wen4    CECA 问

//wen4    E3EB 汶

//wen4    E8B7 璺

//weng1   CECB 嗡

//weng1   CECC 翁

//weng3   DDEE 蓊

//weng4   CECD 瓮

//weng4   DEB3 蕹

//wo1     CECE 挝

//wo1     CECF 蜗

//wo1     CED0 涡*

//wo1     CED1 窝

//wo1     D9C1 倭

//wo1     DDAB 莴*

//wo1     E0B8 喔*

//wo3     CED2 我

//wo4     CED3 斡

//wo4     CED4 卧

//wo4     CED5 握

//wo4     CED6 沃

//wo4     E1A2 幄

//wo4     E4D7 渥

//wo4     EBBF 肟

//wo4     EDD2 硪

//wo4     F6BB 龌

//wu1     B6F1 恶*

//wu1     CED7 巫

//wu1     CED8 呜

//wu1     CED9 钨

//wu1     CEDA 乌

//wu1     CEDB 污

//wu1     CEDC 诬

//wu1     CEDD 屋

//wu1     D8A3 兀*

//wu1     DAF9 邬

//wu1     DBD8 圬

//wu1     ECB6 於*

//wu2     CDF6 亡*

//wu2     CEDE 无*

//wu2     CEDF 芜

//wu2     CEE0 梧

//wu2     CEE1 吾

//wu2     CEE2 吴

//wu2     CEE3 毋

//wu2     CEE6 捂*

//wu2     DFED 唔*

//wu2     E4B4 浯

//wu2     F2DA 蜈

//wu2     F7F9 鼯

//wu3     CEE4 武

//wu3     CEE5 五

//wu3     CEE6 捂*

//wu3     CEE7 午

//wu3     CEE8 舞

//wu3     CEE9 伍

//wu3     CEEA 侮

//wu3     D8F5 仵

//wu3     E2D0 庑

//wu3     E2E4 怃

//wu3     E2E8 忤

//wu3     E5C3 迕

//wu3     E5FC 妩

//wu3     EAF5 牾

//wu3     F0C4 鹉

//wu4     B6F1 恶*

//wu4     CEEB 坞

//wu4     CEEC 戊

//wu4     CEED 雾

//wu4     CEEE 晤

//wu4     CEEF 物

//wu4     CEF0 勿

//wu4     CEF1 务

//wu4     CEF2 悟

//wu4     CEF3 误

//wu4     D8A3 兀*

//wu4     DAE3 阢

//wu4     DCCC 芴

//wu4     E5BB 寤

//wu4     E6C4 婺

//wu4     E6F0 骛

//wu4     E8BB 杌

//wu4     ECC9 焐

//wu4     F0CD 鹜

//wu4     F0ED 痦

//wu4     F6C8 鋈

//xi1     C0B0 腊*

//xi1     C6DC 栖*

//xi1     C6E1 漆*

//xi1     CEF4 昔

//xi1     CEF5 熙

//xi1     CEF6 析

//xi1     CEF7 西

//xi1     CEF8 硒

//xi1     CEF9 矽

//xi1     CEFA 晰

//xi1     CEFB 嘻

//xi1     CEFC 吸

//xi1     CEFD 锡

//xi1     CEFE 牺

//xi1     CFA1 稀

//xi1     CFA2 息

//xi1     CFA3 希

//xi1     CFA4 悉

//xi1     CFA5 膝

//xi1     CFA6 夕

//xi1     CFA7 惜

//xi1     CFA8 熄

//xi1     CFA9 烯

//xi1     CFAA 溪

//xi1     CFAB 汐

//xi1     CFAC 犀

//xi1     D9D2 僖

//xi1     D9E2 兮

//xi1     DBAD 郗

//xi1     DCE7 茜*

//xi1     DDBE 菥

//xi1     DEC9 奚

//xi1     DFF1 唏

//xi1     E4BB 浠

//xi1     E4C0 淅

//xi1     E6D2 嬉

//xi1     E9D8 樨

//xi1     EAD8 曦

//xi1     ECA4 欷

//xi1     ECA8 歙*

//xi1     ECE4 熹

//xi1     F0AA 皙

//xi1     F1B6 穸

//xi1     F1D3 裼*

//xi1     F2E1 蜥

//xi1     F3A3 螅

//xi1     F3AC 蟋

//xi1     F4B8 舾

//xi1     F4CB 羲

//xi1     F4D1 粞

//xi1     F4E2 翕

//xi1     F5B5 醯

//xi1     F5E8 蹊*

//xi1     F7FB 鼷

//xi2     CFAD 檄

//xi2     CFAE 袭

//xi2     CFAF 席

//xi2     CFB0 习

//xi2     CFB1 媳

//xi2     DAF4 隰

//xi2     EAEA 觋

//xi3     CFB2 喜

//xi3     CFB3 铣*

//xi3     CFB4 洗*

//xi3     DDDF 葸

//xi3     DDFB 蓰

//xi3     E1E3 徙

//xi3     E5EF 屣

//xi3     E7F4 玺

//xi3     ECFB 禧

//xi4     CFB5 系*

//xi4     CFB6 隙

//xi4     CFB7 戏*

//xi4     CFB8 细

//xi4     E2BE 饩

//xi4     E3D2 阋

//xi4     ECF9 禊

//xi4     F4AA 舄

//xia1    CFB9 瞎

//xia1    CFBA 虾*

//xia1    DFC8 呷

//xia2    CFBB 匣

//xia2    CFBC 霞

//xia2    CFBD 辖

//xia2    CFBE 暇

//xia2    CFBF 峡

//xia2    CFC0 侠

//xia2    CFC1 狭

//xia2    E1F2 狎

//xia2    E5DA 遐

//xia2    E8A6 瑕

//xia2    E8D4 柙

//xia2    EDCC 硖

//xia2    F7EF 黠

//xia4    BBA3 唬*

//xia4    CFC2 下

//xia4    CFC3 厦*

//xia4    CFC4 夏

//xia4    CFC5 吓*

//xia4    F3C1 罅

//xian1   CFC6 掀

//xian1   CFC7 锨

//xian1   CFC8 先

//xian1   CFC9 仙

//xian1   CFCA 鲜*

//xian1   CFCB 纤*

//xian1   DDB2 莶

//xian1   E5DF 暹

//xian1   EBAF 氙

//xian1   ECEC 祆

//xian1   F4CC 籼

//xian1   F5A3 酰

//xian1   F5D1 跹

//xian2   CFCC 咸

//xian2   CFCD 贤

//xian2   CFCE 衔

//xian2   CFCF 舷

//xian2   CFD0 闲

//xian2   CFD1 涎

//xian2   CFD2 弦

//xian2   CFD3 嫌

//xian2   E6B5 娴

//xian2   F0C2 鹇

//xian2   F0EF 痫

//xian3   CFB3 铣*

//xian3   CFB4 洗*

//xian3   CFCA 鲜*

//xian3   CFD4 显

//xian3   CFD5 险

//xian3   D9FE 冼

//xian3   DEBA 藓

//xian3   E1FD 猃

//xian3   ECDE 燹

//xian3   F2B9 蚬

//xian3   F3DA 筅

//xian3   F5D0 跣

//xian4   BCFB 见*

//xian4   CFD6 现

//xian4   CFD7 献

//xian4   CFD8 县

//xian4   CFD9 腺

//xian4   CFDA 馅

//xian4   CFDB 羡

//xian4   CFDC 宪

//xian4   CFDD 陷

//xian4   CFDE 限

//xian4   CFDF 线

//xian4   DCC8 苋

//xian4   E1AD 岘

//xian4   F6B1 霰

//xiang1  CFE0 相*

//xiang1  CFE1 厢

//xiang1  CFE2 镶

//xiang1  CFE3 香

//xiang1  CFE4 箱

//xiang1  CFE5 襄

//xiang1  CFE6 湘

//xiang1  CFE7 乡

//xiang1  DCBC 芗

//xiang1  DDD9 葙

//xiang1  E6F8 骧

//xiang1  E7BD 缃

//xiang2  BDB5 降*

//xiang2  CFE8 翔

//xiang2  CFE9 祥

//xiang2  CFEA 详

//xiang2  E2D4 庠

//xiang3  CFEB 想

//xiang3  CFEC 响

//xiang3  CFED 享

//xiang3  E2C3 饷

//xiang3  F6DF 鲞

//xiang3  F7CF 飨

//xiang4  CFE0 相*

//xiang4  CFEE 项

//xiang4  CFEF 巷*

//xiang4  CFF0 橡

//xiang4  CFF1 像

//xiang4  CFF2 向

//xiang4  CFF3 象

//xiang4  F3AD 蟓

//xiao1   CFF4 萧

//xiao1   CFF5 硝

//xiao1   CFF6 霄

//xiao1   CFF7 削*

//xiao1   CFF9 嚣

//xiao1   CFFA 销

//xiao1   CFFB 消

//xiao1   CFFC 宵

//xiao1   D0A4 肖*

//xiao1   DFD8 哓

//xiao1   E4EC 潇

//xiao1   E5D0 逍

//xiao1   E6E7 骁

//xiao1   E7AF 绡

//xiao1   E8C9 枭

//xiao1   E8D5 枵

//xiao1   F2D9 蛸*

//xiao1   F3EF 箫

//xiao1   F7CC 魈

//xiao2   CFFD 淆

//xiao2   E1C5 崤

//xiao3   CFFE 晓

//xiao3   D0A1 小

//xiao3   F3E3 筱

//xiao4   CFF8 哮

//xiao4   D0A2 孝

//xiao4   D0A3 校*

//xiao4   D0A4 肖*

//xiao4   D0A5 啸

//xiao4   D0A6 笑

//xiao4   D0A7 效

//xie1    D0A8 楔

//xie1    D0A9 些

//xie1    D0AA 歇

//xie1    D0AB 蝎

//xie2    D0AC 鞋

//xie2    D0AD 协

//xie2    D0AE 挟*

//xie2    D0AF 携

//xie2    D0B0 邪*

//xie2    D0B1 斜

//xie2    D0B2 胁

//xie2    D0B3 谐

//xie2    D2B6 叶*

//xie2    D9C9 偕

//xie2    DBC4 勰

//xie2    DFA2 撷

//xie2    E7D3 缬

//xie2    F2A1 颉*

//xie3    D0B4 写

//xie3    D1AA 血*

//xie4    BDE2 解*

//xie4    C6FC 泣*

//xie4    D0B5 械

//xie4    D0B6 卸

//xie4    D0B7 蟹

//xie4    D0B8 懈

//xie4    D0B9 泄

//xie4    D0BA 泻

//xie4    D0BB 谢

//xie4    D0BC 屑

//xie4    D9F4 亵

//xie4    DBC6 燮

//xie4    DEAF 薤

//xie4    E2B3 獬

//xie4    E2DD 廨

//xie4    E4CD 渫

//xie4    E5AC 瀣

//xie4    E5E2 邂

//xie4    E7A5 绁

//xie4    E9BF 榭

//xie4    E9C7 榍

//xie4    F5F3 躞

//xin1    D0BD 薪

//xin1    D0BE 芯*

//xin1    D0BF 锌

//xin1    D0C0 欣

//xin1    D0C1 辛

//xin1    D0C2 新

//xin1    D0C3 忻

//xin1    D0C4 心

//xin1    DCB0 馨

//xin1    DDB7 莘*

//xin1    EABF 昕

//xin1    ECA7 歆

//xin1    F6CE 鑫

//xin2    EFE2 镡*

//xin4    D0BE 芯*

//xin4    D0C5 信

//xin4    D0C6 衅

//xin4    D8B6 囟

//xing1   D0C7 星

//xing1   D0C8 腥

//xing1   D0C9 猩

//xing1   D0CA 惺

//xing1   D0CB 兴*

//xing2   D0CC 刑

//xing2   D0CD 型

//xing2   D0CE 形

//xing2   D0CF 邢

//xing2   D0D0 行*

//xing2   DAEA 陉

//xing2   DCFE 荥*

//xing2   E2BC 饧*

//xing2   EDCA 硎

//xing3   CAA1 省*

//xing3   D0D1 醒

//xing3   DFA9 擤

//xing4   D0CB 兴*

//xing4   D0D2 幸

//xing4   D0D3 杏

//xing4   D0D4 性

//xing4   D0D5 姓

//xing4   DCF4 荇

//xing4   E3AC 悻

//xiong1  D0D6 兄

//xiong1  D0D7 凶

//xiong1  D0D8 胸

//xiong1  D0D9 匈

//xiong1  D0DA 汹

//xiong1  DCBA 芎

//xiong2  D0DB 雄

//xiong2  D0DC 熊

//xiu1    D0DD 休

//xiu1    D0DE 修

//xiu1    D0DF 羞

//xiu1    DFDD 咻

//xiu1    E2CA 馐

//xiu1    E2D3 庥

//xiu1    F0BC 鸺

//xiu1    F5F7 貅

//xiu1    F7DB 髹

//xiu3    CBDE 宿*

//xiu3    D0E0 朽

//xiu4    B3F4 臭*

//xiu4    CBDE 宿*

//xiu4    D0E1 嗅

//xiu4    D0E2 锈

//xiu4    D0E3 秀

//xiu4    D0E4 袖

//xiu4    D0E5 绣

//xiu4    E1B6 岫

//xiu4    E4E5 溴

//xu1     D0E6 墟

//xu1     D0E7 戌*

//xu1     D0E8 需

//xu1     D0E9 虚

//xu1     D0EA 嘘*

//xu1     D0EB 须

//xu1     D3F5 吁*

//xu1     DBD7 圩*

//xu1     E7EF 顼

//xu1     EDEC 盱

//xu1     F1E3 胥

//xu2     D0EC 徐

//xu3     D0ED 许

//xu3     DABC 诩

//xu3     E4B0 浒*

//xu3     E8F2 栩

//xu3     F4DA 糈

//xu3     F5AF 醑

//xu4     D0EE 蓄

//xu4     D0EF 酗

//xu4     D0F0 叙

//xu4     D0F1 旭

//xu4     D0F2 序

//xu4     D0F3 畜*

//xu4     D0F4 恤

//xu4     D0F5 絮

//xu4     D0F6 婿

//xu4     D0F7 绪

//xu4     D0F8 续

//xu4     DBC3 勖

//xu4     E4AA 洫

//xu4     E4D3 溆

//xu4     ECE3 煦

//xu5     DEA3 蓿

//xuan1   D0F9 轩

//xuan1   D0FA 喧

//xuan1   D0FB 宣

//xuan1   D9D8 儇

//xuan1   DACE 谖

//xuan1   DDE6 萱

//xuan1   DEEF 揎

//xuan1   EAD1 暄

//xuan1   ECD3 煊

//xuan2   D0FC 悬

//xuan2   D0FD 旋*

//xuan2   D0FE 玄

//xuan2   E4F6 漩

//xuan2   E8AF 璇

//xuan2   F0E7 痃

//xuan3   D1A1 选

//xuan3   D1A2 癣

//xuan4   C8AF 券*

//xuan4   D0FD 旋*

//xuan4   D1A3 眩

//xuan4   D1A4 绚

//xuan4   E3F9 泫

//xuan4   E4D6 渲

//xuan4   E9B8 楦

//xuan4   ECC5 炫

//xuan4   EDDB 碹

//xuan4   EEE7 铉

//xuan4   EFE0 镟

//xue1    CFF7 削*

//xue1    D1A5 靴

//xue1    D1A6 薛

//xue2    D1A7 学

//xue2    D1A8 穴

//xue2    E0E5 噱*

//xue2    EDB4 泶

//xue2    F5BD 踅

//xue3    D1A9 雪

//xue3    F7A8 鳕

//xue4    D1AA 血*

//xue4    DACA 谑

//xun1    D1AB 勋

//xun1    D1AC 熏*

//xun1    DBF7 埙

//xun1    DEB9 薰

//xun1    E2B4 獯

//xun1    EAD6 曛

//xun1    F1BF 窨*

//xun1    F5B8 醺

//xun2    D1AD 循

//xun2    D1AE 旬

//xun2    D1AF 询

//xun2    D1B0 寻

//xun2    D1B2 巡

//xun2    DBA8 郇*

//xun2    DCF7 荀

//xun2    DDA1 荨*

//xun2    E1BE 峋

//xun2    E2FE 恂

//xun2    E4AD 洵

//xun2    E4B1 浔

//xun2    F6E0 鲟

//xun4    BFA3 浚*

//xun4    D1AC 熏*

//xun4    D1B1 驯

//xun4    D1B3 殉

//xun4    D1B4 汛

//xun4    D1B5 训

//xun4    D1B6 讯

//xun4    D1B7 逊

//xun4    D1B8 迅

//xun4    D9E3 巽

//xun4    DEA6 蕈

//xun4    E1DF 徇

//ya1     D1B9 压

//ya1     D1BA 押

//ya1     D1BB 鸦

//ya1     D1BC 鸭

//ya1     D1BD 呀*

//ya1     D1BE 丫

//ya1     D1C5 雅*

//ya1     D1C6 哑*

//ya1     DBEB 垭*

//ya1     E8E2 桠

//ya2     D1BF 芽

//ya2     D1C0 牙

//ya2     D1C1 蚜

//ya2     D1C2 崖

//ya2     D1C3 衙

//ya2     D1C4 涯

//ya2     D8F3 伢

//ya2     E1AC 岈

//ya2     E7F0 琊

//ya2     EDFD 睚

//ya3     D1C5 雅*

//ya3     D1C6 哑*

//ya3     F0E9 痖

//ya3     F1E2 疋*

//ya4     D1C7 亚

//ya4     D1C8 讶

//ya4     D4FE 轧*

//ya4     DBEB 垭*

//ya4     DEEB 揠

//ya4     E5C2 迓

//ya4     E6AB 娅

//ya4     EBB2 氩

//ya4     EDBC 砑

//ya5     D1BD 呀*

//yan1    D1C9 焉

//yan1    D1CA 咽*

//yan1    D1CB 阉

//yan1    D1CC 烟

//yan1    D1CD 淹

//yan1    D1E0 燕*

//yan1    D2F3 殷*

//yan1    DBB3 鄢

//yan1    DDCE 菸

//yan1    E1C3 崦

//yan1    E2FB 恹

//yan1    E3D5 阏*

//yan1    E4CE 湮*

//yan1    E6CC 嫣

//yan1    EBD9 胭

//yan1    EBE7 腌*

//yan2    C7A6 铅*

//yan2    D1CE 盐

//yan2    D1CF 严

//yan2    D1D0 研

//yan2    D1D1 蜒

//yan2    D1D2 岩

//yan2    D1D3 延

//yan2    D1D4 言

//yan2    D1D5 颜

//yan2    D1D6 阎

//yan2    D1D7 炎

//yan2    D1D8 沿

//yan2    DAE7 阽*

//yan2    DCBE 芫*

//yan2    E3C6 闫

//yan2    E5FB 妍

//yan2    E9DC 檐

//yan2    F3DB 筵

//yan3    D1D9 奄

//yan3    D1DA 掩

//yan3    D1DB 眼

//yan3    D1DC 衍

//yan3    D1DD 演

//yan3    D8C9 厣

//yan3    D8DF 剡*

//yan3    D9B2 俨

//yan3    D9C8 偃

//yan3    D9F0 兖

//yan3    DBB1 郾

//yan3    E7FC 琰

//yan3    EEBB 罨

//yan3    F7CA 魇

//yan3    F7FA 鼹

//yan4    D1CA 咽*

//yan4    D1DE 艳

//yan4    D1DF 堰

//yan4    D1E0 燕*

//yan4    D1E1 厌

//yan4    D1E2 砚

//yan4    D1E3 雁

//yan4    D1E4 唁

//yan4    D1E5 彦

//yan4    D1E6 焰

//yan4    D1E7 宴

//yan4    D1E8 谚

//yan4    D1E9 验

//yan4    D8CD 赝

//yan4    DADD 谳

//yan4    E4D9 滟

//yan4    EACC 晏

//yan4    ECCD 焱

//yan4    F5A6 酽

//yan4    F7D0 餍

//yang1   D1EA 殃

//yang1   D1EB 央

//yang1   D1EC 鸯

//yang1   D1ED 秧

//yang1   E3F3 泱

//yang1   F7B1 鞅*

//yang2   D1EE 杨

//yang2   D1EF 扬

//yang2   D1F0 佯

//yang2   D1F1 疡

//yang2   D1F2 羊

//yang2   D1F3 洋

//yang2   D1F4 阳

//yang2   E1E0 徉

//yang2   ECBE 炀

//yang2   ECC8 烊*

//yang2   F2D5 蛘

//yang3   D1F5 氧

//yang3   D1F6 仰

//yang3   D1F7 痒

//yang3   D1F8 养

//yang4   D1F9 样

//yang4   D1FA 漾

//yang4   E2F3 怏

//yang4   ECC8 烊*

//yang4   EDA6 恙

//yang4   F7B1 鞅*

//yao1    C3B4 么*

//yao1    D1FB 邀

//yao1    D1FC 腰

//yao1    D1FD 妖

//yao1    D2AA 要*

//yao1    D4BC 约*

//yao1    D8B2 夭

//yao1    DFBA 吆

//yao1    E7DB 幺

//yao2    BDC4 侥*

//yao2    CCD5 陶*

//yao2    D1FE 瑶

//yao2    D2A1 摇

//yao2    D2A2 尧

//yao2    D2A3 遥

//yao2    D2A4 窑

//yao2    D2A5 谣

//yao2    D2A6 姚

//yao2    D8B3 爻

//yao2    E1E6 徭

//yao2    E7F2 珧

//yao2    E9F7 轺

//yao2    EBC8 肴

//yao2    EFA2 铫*

//yao2    F4ED 繇*

//yao2    F7A5 鳐

//yao3    D2A7 咬

//yao3    D2A8 舀

//yao3    E1CA 崾

//yao3    E8C3 杳

//yao3    F1BA 窈

//yao4    C5B1 疟*

//yao4    D2A9 药

//yao4    D2AA 要*

//yao4    D2AB 耀

//yao4    D4BF 钥*

//yao4    EAD7 曜

//yao4    F0CE 鹞

//ye1     D2AC 椰

//ye1     D2AD 噎

//ye1     D2AE 耶*

//ye1     D2B4 掖*

//ye2     D0B0 邪*

//ye2     D2AE 耶*

//ye2     D2AF 爷

//ye2     DEDE 揶

//ye2     EEF4 铘

//ye3     D2B0 野

//ye3     D2B1 冶

//ye3     D2B2 也

//ye4     D1CA 咽*

//ye4     D2B3 页

//ye4     D2B4 掖*

//ye4     D2B5 业

//ye4     D2B6 叶*

//ye4     D2B7 曳

//ye4     D2B8 腋

//ye4     D2B9 夜

//ye4     D2BA 液

//ye4     D7A7 拽*

//ye4     D8CC 靥

//ye4     DACB 谒

//ye4     DAFE 邺

//ye4     EACA 晔

//ye4     ECC7 烨

//yi1     D2BB 一

//yi1     D2BC 壹

//yi1     D2BD 医

//yi1     D2BE 揖

//yi1     D2BF 铱

//yi1     D2C0 依

//yi1     D2C1 伊

//yi1     D2C2 衣*

//yi1     D2CE 椅*

//yi1     DFDE 咿

//yi1     E0E6 噫

//yi1     E2A2 猗

//yi1     E4F4 漪

//yi1     ECA5 欹*

//yi1     F7F0 黟

//yi2     C9DF 蛇*

//yi2     D2C3 颐

//yi2     D2C4 夷

//yi2     D2C5 遗*

//yi2     D2C6 移

//yi2     D2C7 仪

//yi2     D2C8 胰

//yi2     D2C9 疑

//yi2     D2CA 沂

//yi2     D2CB 宜

//yi2     D2CC 姨

//yi2     D2CD 彝

//yi2     DAB1 诒

//yi2     DBDD 圯

//yi2     DCE8 荑*

//yi2     DFD7 咦

//yi2     E1DA 嶷

//yi2     E2C2 饴

//yi2     E2F9 怡

//yi2     E5C6 迤*

//yi2     EADD 贻

//yi2     EDF4 眙

//yi2     F0EA 痍

//yi3     CEB2 尾*

//yi3     D2CE 椅*

//yi3     D2CF 蚁

//yi3     D2D0 倚

//yi3     D2D1 已

//yi3     D2D2 乙

//yi3     D2D3 矣

//yi3     D2D4 以

//yi3     DCD3 苡

//yi3     E5C6 迤*

//yi3     ECBD 旖

//yi3     EEC6 钇

//yi3     F4AF 舣

//yi3     F4FD 酏

//yi4     B0AC 艾*

//yi4     D2C2 衣*

//yi4     D2D5 艺

//yi4     D2D6 抑

//yi4     D2D7 易

//yi4     D2D8 邑

//yi4     D2D9 屹*

//yi4     D2DA 亿

//yi4     D2DB 役

//yi4     D2DC 臆

//yi4     D2DD 逸

//yi4     D2DE 肄

//yi4     D2DF 疫

//yi4     D2E0 亦

//yi4     D2E1 裔

//yi4     D2E2 意

//yi4     D2E3 毅

//yi4     D2E4 忆

//yi4     D2E5 义

//yi4     D2E6 益

//yi4     D2E7 溢

//yi4     D2E8 诣

//yi4     D2E9 议

//yi4     D2EA 谊

//yi4     D2EB 译

//yi4     D2EC 异

//yi4     D2ED 翼

//yi4     D2EE 翌

//yi4     D2EF 绎

//yi4     D8D7 刈

//yi4     D8E6 劓

//yi4     D8FD 佚

//yi4     D9AB 佾

//yi4     DBFC 埸

//yi4     DCB2 懿

//yi4     DEB2 薏

//yi4     DEC4 弈

//yi4     DEC8 奕

//yi4     DEDA 挹

//yi4     DFAE 弋

//yi4     DFBD 呓

//yi4     E0C9 嗌*

//yi4     E1BB 峄

//yi4     E2F8 怿

//yi4     E3A8 悒

//yi4     E6E4 驿

//yi4     E7CB 缢

//yi4     E9EC 殪

//yi4     E9F3 轶

//yi4     ECDA 熠

//yi4     EFD7 镒

//yi4     EFEE 镱

//yi4     F0F9 瘗

//yi4     F1AF 癔

//yi4     F1B4 翊

//yi4     F2E6 蜴

//yi4     F4E0 羿

//yi4     F4E8 翳

//yin1    D2F0 茵

//yin1    D2F1 荫*

//yin1    D2F2 因

//yin1    D2F3 殷*

//yin1    D2F4 音

//yin1    D2F5 阴

//yin1    D2F6 姻

//yin1    DCA7 堙

//yin1    E0B3 喑

//yin1    E4A6 洇

//yin1    E4CE 湮*

//yin1    EBB3 氤

//yin1    EEF7 铟

//yin2    D2F7 吟

//yin2    D2F8 银

//yin2    D2F9 淫

//yin2    D2FA 寅

//yin2    DBB4 鄞

//yin2    DBF3 垠

//yin2    E1FE 狺

//yin2    E2B9 夤

//yin2    F6AF 霪

//yin2    F6B8 龈*

//yin3    D2FB 饮*

//yin3    D2FC 尹

//yin3    D2FD 引

//yin3    D2FE 隐

//yin3    DFC5 吲

//yin3    F1AB 瘾

//yin3    F2BE 蚓

//yin4    D2F1 荫*

//yin4    D2FB 饮*

//yin4    D3A1 印

//yin4    D8B7 胤

//yin4    DCE1 茚

//yin4    F1BF 窨*

//ying1   D3A2 英

//ying1   D3A3 樱

//ying1   D3A4 婴

//ying1   D3A5 鹰

//ying1   D3A6 应*

//ying1   D3A7 缨

//ying1   DDBA 莺

//ying1   DEFC 撄

//ying1   E0D3 嘤

//ying1   E2DF 膺

//ying1   E7F8 瑛

//ying1   E8AC 璎

//ying1   F0D0 鹦

//ying1   F3BF 罂

//ying2   D3A8 莹

//ying2   D3A9 萤

//ying2   D3AA 营

//ying2   D3AB 荧

//ying2   D3AC 蝇

//ying2   D3AD 迎

//ying2   D3AE 赢

//ying2   D3AF 盈

//ying2   D9F8 嬴

//ying2   DCE3 茔

//ying2   DCFE 荥*

//ying2   DDD3 萦

//ying2   DDF6 蓥

//ying2   E4DE 滢

//ying2   E4EB 潆

//ying2   E5AD 瀛

//ying2   E9BA 楹

//ying3   D3B0 影

//ying3   D3B1 颖

//ying3   DBAB 郢

//ying3   F1A8 瘿

//ying3   F2A3 颍

//ying4   D3A6 应*

//ying4   D3B2 硬

//ying4   D3B3 映

//ying4   EBF4 媵

//yo1     D3B4 哟*

//yo1     D3FD 育*

//yo1     E0A1 唷

//yo5     D3B4 哟*

//yong1   D3B5 拥

//yong1   D3B6 佣*

//yong1   D3B7 臃

//yong1   D3B8 痈

//yong1   D3B9 庸

//yong1   D3BA 雍

//yong1   DBD5 壅

//yong1   DCAD 墉

//yong1   E3BC 慵

//yong1   E7DF 邕

//yong1   EFDE 镛

//yong1   F7AB 鳙

//yong1   F7D3 饔

//yong2   E0AF 喁

//yong3   D3BB 踊

//yong3   D3BC 蛹

//yong3   D3BD 咏

//yong3   D3BE 泳

//yong3   D3BF 涌*

//yong3   D3C0 永

//yong3   D3C1 恿

//yong3   D3C2 勇

//yong3   D9B8 俑

//yong3   F0AE 甬

//yong4   D3B6 佣*

//yong4   D3C3 用

//you1    D3C4 幽

//you1    D3C5 优

//you1    D3C6 悠

//you1    D3C7 忧

//you1    D8FC 攸

//you1    DFCF 呦

//you2    D3C8 尤

//you2    D3C9 由

//you2    D3CA 邮

//you2    D3CB 铀

//you2    D3CC 犹

//you2    D3CD 油

//you2    D3CE 游

//you2    DDAF 莜

//you2    DDB5 莸

//you2    E8D6 柚*

//you2    E9E0 猷

//you2    F0E0 疣

//you2    F2C4 蚰

//you2    F2F6 蝣

//you2    F2F8 蝤*

//you2    F4ED 繇*

//you2    F6CF 鱿

//you3    D3CF 酉

//you3    D3D0 有*

//you3    D3D1 友

//you3    D8D5 卣

//you3    DDAC 莠

//you3    EBBB 牖

//you3    EEF0 铕

//you3    F7EE 黝

//you4    D3D0 有*

//you4    D3D2 右

//you4    D3D3 佑

//you4    D3D4 釉

//you4    D3D5 诱

//you4    D3D6 又

//you4    D3D7 幼

//you4    D9A7 侑

//you4    E0F3 囿

//you4    E5B6 宥

//you4    E8D6 柚*

//you4    F2CA 蚴

//you4    F7F8 鼬

//yu1     D3D8 迂

//yu1     D3D9 淤

//yu1     E6FA 纡

//yu1     ECB6 於*

//yu1     F0F6 瘀

//yu2     D3DA 于

//yu2     D3DB 盂

//yu2     D3DC 榆

//yu2     D3DD 虞

//yu2     D3DE 愚

//yu2     D3DF 舆

//yu2     D3E0 余

//yu2     D3E1 俞*

//yu2     D3E2 逾

//yu2     D3E3 鱼

//yu2     D3E4 愉

//yu2     D3E5 渝

//yu2     D3E6 渔

//yu2     D3E7 隅

//yu2     D3E8 予*

//yu2     D3E9 娱

//yu2     D3EB 与*

//yu2     D8AE 禺

//yu2     DAC4 谀

//yu2     DDC7 萸

//yu2     DEED 揄

//yu2     E1CE 嵛

//yu2     E1FC 狳

//yu2     E2C5 馀

//yu2     E6A5 妤

//yu2     E8A4 瑜

//yu2     EAEC 觎

//yu2     EBE9 腴

//yu2     ECA3 欤

//yu2     ECB6 於*

//yu2     F1BE 窬

//yu2     F2F5 蝓

//yu2     F3C4 竽

//yu2     F4A7 臾

//yu2     F4A8 舁

//yu2     F6A7 雩

//yu3     D3E8 予*

//yu3     D3EA 雨*

//yu3     D3EB 与*

//yu3     D3EC 屿

//yu3     D3ED 禹

//yu3     D3EE 宇

//yu3     D3EF 语*

//yu3     D3F0 羽

//yu3     D8F1 伛

//yu3     D9B6 俣

//yu3     E0F4 圄

//yu3     E0F6 圉

//yu3     E2D7 庾

//yu3     F0F5 瘐

//yu3     F1C1 窳

//yu3     F6B9 龉

//yu4     B9C8 谷*

//yu4     CEB5 蔚*

//yu4     CEBE 尉*

//yu4     D3EA 雨*

//yu4     D3EB 与*

//yu4     D3EF 语*

//yu4     D3F1 玉

//yu4     D3F2 域

//yu4     D3F3 芋

//yu4     D3F4 郁

//yu4     D3F5 吁*

//yu4     D3F6 遇

//yu4     D3F7 喻

//yu4     D3F8 峪

//yu4     D3F9 御

//yu4     D3FA 愈

//yu4     D3FB 欲

//yu4     D3FC 狱

//yu4     D3FD 育*

//yu4     D3FE 誉

//yu4     D4A1 浴

//yu4     D4A2 寓

//yu4     D4A3 裕

//yu4     D4A4 预

//yu4     D4A5 豫

//yu4     D4A6 驭

//yu4     D6E0 粥*

//yu4     D8B9 毓

//yu4     DACD 谕

//yu4     DDF7 蓣

//yu4     E2C0 饫

//yu4     E3D0 阈

//yu4     E5F7 鬻

//yu4     E5FD 妪

//yu4     EAC5 昱

//yu4     ECCF 煜

//yu4     ECD9 熨*

//yu4     ECDB 燠

//yu4     EDB2 聿

//yu4     EEDA 钰

//yu4     F0C1 鹆

//yu4     F0D6 鹬

//yu4     F2E2 蜮

//yuan1   D4A7 鸳

//yuan1   D4A8 渊

//yuan1   D4A9 冤

//yuan1   EDF3 眢

//yuan1   F0B0 鸢

//yuan1   F3EE 箢

//yuan2   D4AA 元

//yuan2   D4AB 垣

//yuan2   D4AC 袁

//yuan2   D4AD 原

//yuan2   D4AE 援

//yuan2   D4AF 辕

//yuan2   D4B0 园

//yuan2   D4B1 员*

//yuan2   D4B2 圆

//yuan2   D4B3 猿

//yuan2   D4B4 源

//yuan2   D4B5 缘

//yuan2   DCAB 塬

//yuan2   DCBE 芫*

//yuan2   E0F7 圜*

//yuan2   E3E4 沅

//yuan2   E6C2 媛*

//yuan2   E9DA 橼

//yuan2   EBBC 爰

//yuan2   F3A2 螈

//yuan2   F6BD 鼋

//yuan3   D4B6 远

//yuan4   D4B7 苑

//yuan4   D4B8 愿

//yuan4   D4B9 怨

//yuan4   D4BA 院

//yuan4   DBF9 垸

//yuan4   DEF2 掾

//yuan4   E6C2 媛*

//yuan4   E8A5 瑗

//yue1    D4BB 曰

//yue1    D4BC 约*

//yue3    DFDC 哕*

//yue4    C0D6 乐*

//yue4    CBB5 说*

//yue4    D4BD 越

//yue4    D4BE 跃

//yue4    D4BF 钥*

//yue4    D4C0 岳

//yue4    D4C1 粤

//yue4    D4C2 月

//yue4    D4C3 悦

//yue4    D4C4 阅

//yue4    D9DF 龠

//yue4    E5AE 瀹

//yue4    E8DD 栎*

//yue4    E9D0 樾

//yue4    EBBE 刖

//yue4    EEE1 钺

//yun1    D4CE 晕*

//yun1    EBB5 氲

//yun2    D4B1 员*

//yun2    D4C5 耘

//yun2    D4C6 云

//yun2    D4C7 郧

//yun2    D4C8 匀

//yun2    DCBF 芸

//yun2    E7A1 纭

//yun2    EAC0 昀

//yun2    F3DE 筠*

//yun3    D4C9 陨

//yun3    D4CA 允

//yun3    E1F1 狁

//yun3    E9E6 殒

//yun4    D4B1 员*

//yun4    D4CB 运

//yun4    D4CC 蕴

//yun4    D4CD 酝

//yun4    D4CE 晕*

//yun4    D4CF 韵

//yun4    D4D0 孕

//yun4    DBA9 郓

//yun4    E3A2 恽

//yun4    E3B3 愠

//yun4    E8B9 韫

//yun4    ECD9 熨*

//za1     D4D1 匝

//za1     D4FA 扎*

//za1     DED9 拶*

//za1     DFC6 咂

//za2     D4D2 砸

//za2     D4D3 杂

//za2     D4DB 咱*

//za3     D5A6 咋*

//zai1    D4D4 栽*

//zai1    D4D5 哉

//zai1    D4D6 灾

//zai1    E7DE 甾

//zai3    D4D7 宰

//zai3    D4D8 载*

//zai3    D7D0 仔*

//zai3    E1CC 崽

//zai4    D4D8 载*

//zai4    D4D9 再

//zai4    D4DA 在

//zan1    F4A2 簪

//zan1    F4D8 糌

//zan2    D4DB 咱*

//zan3    D4DC 攒*

//zan3    DED9 拶*

//zan3    EAC3 昝

//zan3    F4F5 趱

//zan4    D4DD 暂

//zan4    D4DE 赞

//zan4    E8B6 瓒

//zan4    F6C9 錾

//zan5    D4DB 咱*

//zang1   D4DF 赃

//zang1   D4E0 脏*

//zang1   EAB0 臧

//zang3   E6E0 驵

//zang4   B2D8 藏*

//zang4   D4E0 脏*

//zang4   D4E1 葬

//zang4   DECA 奘*

//zao1    D4E2 遭

//zao1    D4E3 糟

//zao2    D4E4 凿

//zao3    D4E5 藻

//zao3    D4E6 枣

//zao3    D4E7 早

//zao3    D4E8 澡

//zao3    D4E9 蚤

//zao4    D4EA 躁

//zao4    D4EB 噪

//zao4    D4EC 造

//zao4    D4ED 皂

//zao4    D4EE 灶

//zao4    D4EF 燥

//zao4    DFF0 唣

//ze2     D4F0 责

//ze2     D4F1 择*

//ze2     D4F2 则

//ze2     D4F3 泽

//ze2     D8D3 赜

//ze2     DFF5 啧

//ze2     E0FD 帻

//ze2     E5C5 迮

//ze2     F3D0 笮*

//ze2     F3E5 箦

//ze2     F4B7 舴

//ze4     B2E0 侧*

//ze4     D8C6 仄

//ze4     EABE 昃

//zei2    D4F4 贼

//zen3    D4F5 怎

//zen4    DADA 谮

//zeng1   D4F6 增

//zeng1   D4F7 憎

//zeng1   D4F8 曾*

//zeng1   E7D5 缯*

//zeng1   EEC0 罾

//zeng4   D4F9 赠

//zeng4   D7DB 综*

//zeng4   E7D5 缯*

//zeng4   EAB5 甑

//zeng4   EFAD 锃

//zha1    B2E7 茬*

//zha1    B2E9 查*

//zha1    D4FA 扎*

//zha1    D4FB 喳*

//zha1    D4FC 渣

//zha1    D5A6 咋*

//zha1    DEEA 揸

//zha1    DFB8 吒*

//zha1    DFEE 哳

//zha1    F7FE 齄

//zha2    D4FA 扎*

//zha2    D4FD 札*

//zha2    D4FE 轧*

//zha2    D5A1 铡

//zha2    D5A2 闸

//zha2    D5A8 炸*

//zha3    D5A3 眨

//zha3    EDC4 砟

//zha4    C0AF 蜡*

//zha4    D5A4 栅*

//zha4    D5A5 榨

//zha4    D5A6 咋*

//zha4    D5A7 乍

//zha4    D5A8 炸*

//zha4    D5A9 诈

//zha4    D7F5 柞*

//zha4    DFB8 吒*

//zha4    DFE5 咤

//zha4    F0E4 痄

//zha4    F2C6 蚱

//zhai1   B2E0 侧*

//zhai1   D5AA 摘

//zhai1   D5AB 斋

//zhai2   B5D4 翟*

//zhai2   D4F1 择*

//zhai2   D5AC 宅

//zhai3   D5AD 窄

//zhai4   D5AE 债

//zhai4   D5AF 寨

//zhai4   EDCE 砦

//zhai4   F1A9 瘵

//zhan1   D5B0 瞻

//zhan1   D5B1 毡

//zhan1   D5B2 詹

//zhan1   D5B3 粘*

//zhan1   D5B4 沾

//zhan1   D5BC 占*

//zhan1   DADE 谵

//zhan1   ECB9 旃

//zhan3   D5B5 盏

//zhan3   D5B6 斩

//zhan3   D5B7 辗

//zhan3   D5B8 崭

//zhan3   D5B9 展

//zhan3   DEF8 搌

//zhan4   B2FC 颤*

//zhan4   D5BA 蘸

//zhan4   D5BB 栈

//zhan4   D5BC 占*

//zhan4   D5BD 战

//zhan4   D5BE 站

//zhan4   D5BF 湛

//zhan4   D5C0 绽

//zhang1  D5C1 樟

//zhang1  D5C2 章

//zhang1  D5C3 彰

//zhang1  D5C4 漳

//zhang1  D5C5 张

//zhang1  DBB5 鄣

//zhang1  E2AF 獐

//zhang1  E6D1 嫜

//zhang1  E8B0 璋

//zhang1  F3AF 蟑

//zhang3  B3A4 长*

//zhang3  D5C6 掌

//zhang3  D5C7 涨*

//zhang3  D8EB 仉

//zhang4  D5C7 涨*

//zhang4  D5C8 杖

//zhang4  D5C9 丈

//zhang4  D5CA 帐

//zhang4  D5CB 账

//zhang4  D5CC 仗

//zhang4  D5CD 胀

//zhang4  D5CE 瘴

//zhang4  D5CF 障

//zhang4  E1A4 幛

//zhang4  E1D6 嶂

//zhao1   B3AF 朝*

//zhao1   B3B0 嘲*

//zhao1   D5D0 招

//zhao1   D5D1 昭

//zhao1   D7C5 着*

//zhao1   DFFA 啁*

//zhao1   EEC8 钊

//zhao2   D7C5 着*

//zhao3   D5D2 找

//zhao3   D5D3 沼

//zhao3   D7A6 爪*

//zhao4   D5D4 赵

//zhao4   D5D5 照

//zhao4   D5D6 罩

//zhao4   D5D7 兆

//zhao4   D5D8 肇

//zhao4   D5D9 召*

//zhao4   DAAF 诏

//zhao4   E8FE 棹*

//zhao4   F3C9 笊

//zhe1    D5DA 遮

//zhe1    D5DB 折*

//zhe1    F2D8 蜇*

//zhe1    F3A7 螫*

//zhe2    D5DB 折*

//zhe2    D5DC 哲

//zhe2    D5DD 蛰

//zhe2    D5DE 辙

//zhe2    DAD8 谪

//zhe2    DFA1 摺

//zhe2    E9FC 辄

//zhe2    EDDD 磔

//zhe2    F2D8 蜇*

//zhe3    D5DF 者

//zhe3    D5E0 锗

//zhe3    F1DE 褶

//zhe3    F4F7 赭

//zhe4    D5E1 蔗

//zhe4    D5E2 这*

//zhe4    D5E3 浙

//zhe4    E8CF 柘

//zhe4    F0D1 鹧

//zhe5    D7C5 着*

//zhei4   D5E2 这*

//zhen1   D5E4 珍

//zhen1   D5E5 斟

//zhen1   D5E6 真

//zhen1   D5E7 甄

//zhen1   D5E8 砧

//zhen1   D5E9 臻

//zhen1   D5EA 贞

//zhen1   D5EB 针

//zhen1   D5EC 侦

//zhen1   D6A1 帧

//zhen1   DDE8 蓁

//zhen1   E4A5 浈

//zhen1   E4DA 溱*

//zhen1   E8E5 桢

//zhen1   E9BB 榛

//zhen1   EBD3 胗

//zhen1   ECF5 祯

//zhen1   F3F0 箴

//zhen3   D5ED 枕

//zhen3   D5EE 疹

//zhen3   D5EF 诊

//zhen3   E7C7 缜

//zhen3   E9F4 轸

//zhen3   EEB3 畛

//zhen3   F0A1 稹

//zhen4   D5F0 震

//zhen4   D5F1 振

//zhen4   D5F2 镇

//zhen4   D5F3 阵

//zhen4   DBDA 圳

//zhen4   E9A9 椹*

//zhen4   EAE2 赈

//zhen4   EBDE 朕

//zhen4   F0B2 鸩

//zheng1  B6A1 丁*

//zheng1  D5F4 蒸

//zheng1  D5F5 挣*

//zheng1  D5F6 睁

//zheng1  D5F7 征

//zheng1  D5F8 狰

//zheng1  D5F9 争

//zheng1  D5FA 怔

//zheng1  D5FD 正*

//zheng1  D6A2 症*

//zheng1  E1BF 峥

//zheng1  E1E7 徵*

//zheng1  EEDB 钲*

//zheng1  EFA3 铮

//zheng1  F3DD 筝

//zheng3  D5FB 整

//zheng3  D5FC 拯

//zheng4  D5F5 挣*

//zheng4  D5FD 正*

//zheng4  D5FE 政

//zheng4  D6A2 症*

//zheng4  D6A3 郑

//zheng4  D6A4 证

//zheng4  DABA 诤

//zheng4  EEDB 钲*

//zhi1    CACF 氏*

//zhi1    D6A5 芝

//zhi1    D6A6 枝

//zhi1    D6A7 支

//zhi1    D6A8 吱*

//zhi1    D6A9 蜘

//zhi1    D6AA 知

//zhi1    D6AB 肢

//zhi1    D6AC 脂

//zhi1    D6AD 汁

//zhi1    D6AE 之

//zhi1    D6AF 织

//zhi1    D6BB 只*

//zhi1    D8B4 卮

//zhi1    E8D9 栀

//zhi1    EBD5 胝

//zhi1    ECF3 祗

//zhi2    D6B0 职

//zhi2    D6B1 直

//zhi2    D6B2 植

//zhi2    D6B3 殖*

//zhi2    D6B4 执

//zhi2    D6B5 值

//zhi2    D6B6 侄

//zhi2    DBFA 埴

//zhi2    DEFD 摭

//zhi2    F4EA 絷

//zhi2    F5C5 跖

//zhi2    F5DC 踯

//zhi3    D6B7 址

//zhi3    D6B8 指

//zhi3    D6B9 止

//zhi3    D6BA 趾

//zhi3    D6BB 只*

//zhi3    D6BC 旨

//zhi3    D6BD 纸

//zhi3    DCC6 芷

//zhi3    E1E7 徵*

//zhi3    E5EB 咫

//zhi3    E8D7 枳

//zhi3    E9F2 轵

//zhi3    ECED 祉

//zhi3    EDE9 黹

//zhi3    F5A5 酯

//zhi4    CAB6 识*

//zhi4    D6BE 志

//zhi4    D6BF 挚

//zhi4    D6C0 掷

//zhi4    D6C1 至

//zhi4    D6C2 致

//zhi4    D6C3 置

//zhi4    D6C4 帜

//zhi4    D6C5 峙*

//zhi4    D6C6 制

//zhi4    D6C7 智

//zhi4    D6C8 秩

//zhi4    D6C9 稚

//zhi4    D6CA 质

//zhi4    D6CB 炙

//zhi4    D6CC 痔

//zhi4    D6CD 滞

//zhi4    D6CE 治

//zhi4    D6CF 窒

//zhi4    DAEC 陟

//zhi4    DBA4 郅

//zhi4    E0F9 帙

//zhi4    E2E5 忮

//zhi4    E5E9 彘

//zhi4    E6EF 骘

//zhi4    E8CE 栉

//zhi4    E8E4 桎

//zhi4    E9F9 轾

//zhi4    EADE 贽

//zhi4    EBF9 膣

//zhi4    EFF4 雉

//zhi4    F0BA 鸷

//zhi4    F0EB 痣

//zhi4    F2CE 蛭

//zhi4    F5D9 踬

//zhi4    F5F4 豸

//zhi4    F6A3 觯

//zhong1  D6D0 中*

//zhong1  D6D1 盅

//zhong1  D6D2 忠

//zhong1  D6D3 钟

//zhong1  D6D4 衷

//zhong1  D6D5 终

//zhong1  E2EC 忪*

//zhong1  EFF1 锺

//zhong1  F3AE 螽

//zhong1  F4B1 舯

//zhong3  D6D6 种*

//zhong3  D6D7 肿

//zhong3  DAA3 冢

//zhong3  F5E0 踵

//zhong4  D6D0 中*

//zhong4  D6D6 种*

//zhong4  D6D8 重*

//zhong4  D6D9 仲

//zhong4  D6DA 众

//zhou1   D6DB 舟

//zhou1   D6DC 周

//zhou1   D6DD 州

//zhou1   D6DE 洲

//zhou1   D6DF 诌

//zhou1   D6E0 粥*

//zhou1   DFFA 啁*

//zhou2   D6E1 轴*

//zhou2   E6A8 妯

//zhou2   EDD8 碡

//zhou3   D6E2 肘

//zhou3   D6E3 帚

//zhou4   D6E1 轴*

//zhou4   D6E4 咒

//zhou4   D6E5 皱

//zhou4   D6E6 宙

//zhou4   D6E7 昼

//zhou4   D6E8 骤

//zhou4   DDA7 荮

//zhou4   E6FB 纣

//zhou4   E7A7 绉

//zhou4   EBD0 胄

//zhou4   F4A6 籀

//zhou4   F4FC 酎

//zhu1    D6E9 珠

//zhu1    D6EA 株

//zhu1    D6EB 蛛

//zhu1    D6EC 朱

//zhu1    D6ED 猪

//zhu1    D6EE 诸

//zhu1    D6EF 诛

//zhu1    D9AA 侏

//zhu1    DBA5 邾

//zhu1    DCEF 茱

//zhu1    E4A8 洙

//zhu1    E4F3 潴

//zhu1    E9C6 槠

//zhu1    E9CD 橥

//zhu1    EEF9 铢

//zhu2    CAF5 术*

//zhu2    D6F0 逐

//zhu2    D6F1 竹

//zhu2    D6F2 烛

//zhu2    D6FE 筑

//zhu2    F0F1 瘃

//zhu2    F3C3 竺

//zhu2    F4B6 舳

//zhu2    F5EE 躅

//zhu3    CAF4 属*

//zhu3    D6F3 煮

//zhu3    D6F4 拄

//zhu3    D6F5 瞩

//zhu3    D6F6 嘱

//zhu3    D6F7 主

//zhu3    E4BE 渚

//zhu3    F7E6 麈

//zhu4    D6F8 著*

//zhu4    D6F9 柱

//zhu4    D6FA 助

//zhu4    D6FB 蛀

//zhu4    D6FC 贮

//zhu4    D6FD 铸

//zhu4    D7A1 住

//zhu4    D7A2 注

//zhu4    D7A3 祝

//zhu4    D7A4 驻

//zhu4    D8F9 伫

//zhu4    DCD1 苎

//zhu4    E8CC 杼

//zhu4    ECC4 炷

//zhu4    F0E6 疰

//zhu4    F3E7 箸

//zhu4    F4E3 翥

//zhua1   D7A5 抓

//zhua1   DDAB 莴*

//zhua3   D7A6 爪*

//zhuai1  D7A7 拽*

//zhuai3  D7AA 转*

//zhuai4  D7A7 拽*

//zhuan1  D7A8 专

//zhuan1  D7A9 砖

//zhuan1  F2A7 颛

//zhuan3  D7AA 转*

//zhuan4  B4AB 传*

//zhuan4  D7AA 转*

//zhuan4  D7AB 撰

//zhuan4  D7AC 赚*

//zhuan4  D7AD 篆

//zhuan4  DFF9 啭

//zhuan4  E2CD 馔

//zhuang1 D7AE 桩

//zhuang1 D7AF 庄

//zhuang1 D7B0 装

//zhuang1 D7B1 妆

//zhuang3 DECA 奘*

//zhuang4 B4B1 幢*

//zhuang4 D7B2 撞

//zhuang4 D7B3 壮

//zhuang4 D7B4 状

//zhuang4 D9D7 僮*

//zhuang4 EDB0 戆*

//zhui1   D7B5 椎*

//zhui1   D7B6 锥

//zhui1   D7B7 追

//zhui1   E6ED 骓

//zhui1   F6BF 隹

//zhui4   D7B8 赘

//zhui4   D7B9 坠

//zhui4   D7BA 缀

//zhui4   E3B7 惴

//zhui4   E7C4 缒

//zhun1   CDCD 屯*

//zhun1   D7BB 谆

//zhun1   EBC6 肫

//zhun1   F1B8 窀

//zhun3   D7BC 准

//zhuo1   D7BD 捉

//zhuo1   D7BE 拙

//zhuo1   D7C0 桌

//zhuo1   D9BE 倬

//zhuo1   E4C3 涿

//zhuo1   E8FE 棹*

//zhuo1   ECCC 焯*

//zhuo2   BDC9 缴*

//zhuo2   D6F8 著*

//zhuo2   D7BF 卓

//zhuo2   D7C1 琢

//zhuo2   D7C2 茁

//zhuo2   D7C3 酌

//zhuo2   D7C4 啄

//zhuo2   D7C5 着*

//zhuo2   D7C6 灼

//zhuo2   D7C7 浊

//zhuo2   DAC2 诼

//zhuo2   DFAA 擢

//zhuo2   E4B7 浞

//zhuo2   E5AA 濯

//zhuo2   ECFA 禚

//zhuo2   EDBD 斫

//zhuo2   EFED 镯

//zi1     D6A8 吱*

//zi1     D7C8 兹*

//zi1     D7C9 咨

//zi1     D7CA 资

//zi1     D7CB 姿

//zi1     D7CC 滋

//zi1     D7CD 淄

//zi1     D7CE 孜

//zi1     D7D0 仔*

//zi1     DAD1 谘

//zi1     E1D1 嵫

//zi1     E6DC 孳

//zi1     E7BB 缁

//zi1     EAA2 辎

//zi1     EADF 赀

//zi1     EFC5 锱

//zi1     F4D2 粢

//zi1     F4F4 趑

//zi1     F5FE 觜*

//zi1     F6A4 訾*

//zi1     F6B7 龇

//zi1     F6F6 鲻

//zi1     F7DA 髭

//zi3     D7CF 紫

//zi3     D7D0 仔*

//zi3     D7D1 籽

//zi3     D7D2 滓

//zi3     D7D3 子

//zi3     DCEB 茈*

//zi3     E6A2 姊

//zi3     E8F7 梓

//zi3     EFF6 秭

//zi3     F1E8 耔

//zi3     F3CA 笫

//zi3     F6A4 訾*

//zi4     D7D4 自

//zi4     D7D5 渍

//zi4     D7D6 字

//zi4     EDA7 恣

//zi4     EDF6 眦

//zong1   D7D7 鬃

//zong1   D7D8 棕

//zong1   D7D9 踪

//zong1   D7DA 宗

//zong1   D7DB 综*

//zong1   E8C8 枞*

//zong1   EBEA 腙

//zong3   D7DC 总

//zong3   D9CC 偬

//zong4   D7DD 纵

//zong4   F4D5 粽

//zou1    D7DE 邹

//zou1    DAC1 诹

//zou1    DAEE 陬

//zou1    DBB8 鄹

//zou1    E6E3 驺

//zou1    F6ED 鲰

//zou3    D7DF 走

//zou4    D7E0 奏

//zou4    D7E1 揍

//zu1     D7E2 租

//zu1     DDCF 菹

//zu2     D7E3 足

//zu2     D7E4 卒*

//zu2     D7E5 族

//zu2     EFDF 镞

//zu3     D7E6 祖

//zu3     D7E7 诅

//zu3     D7E8 阻

//zu3     D7E9 组

//zu3     D9DE 俎

//zuan1   D7EA 钻*

//zuan1   F5F2 躜

//zuan3   D7EB 纂

//zuan3   E7DA 缵

//zuan4   D7AC 赚*

//zuan4   D7EA 钻*

//zuan4   DFAC 攥

//zui3    BED7 咀*

//zui3    D7EC 嘴

//zui3    F5FE 觜*

//zui4    D7ED 醉

//zui4    D7EE 最

//zui4    D7EF 罪

//zui4    DEA9 蕞

//zun1    D7F0 尊

//zun1    D7F1 遵

//zun1    E9D7 樽

//zun1    F7AE 鳟

//zun3    DFA4 撙

//zuo1    D7F7 作*

//zuo1    E0DC 嘬*

//zuo2    D7F2 昨

//zuo2    F3D0 笮*

//zuo3    B4E9 撮*

//zuo3    D7F3 左

//zuo3    D7F4 佐

//zuo4    D7F5 柞*

//zuo4    D7F6 做

//zuo4    D7F7 作*

//zuo4    D7F8 坐

//zuo4    D7F9 座

//zuo4    DAE8 阼

//zuo4    DFF2 唑

//zuo4    E2F4 怍

//zuo4    EBD1 胙

//zuo4    ECF1 祚

//zuo4    F5A1 酢*
//            ";
//#endif
//    }
//}

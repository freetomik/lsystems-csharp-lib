using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LSystems
{
    public sealed class ProductionAttribute : Attribute
    {
        public ProductionAttribute(string schema)
        {
            this.Schema = schema;
        }

        public ProductionAttribute()
        {
            this.Schema = string.Empty;
        }

        public string Schema { get; set; }

        private static Regex productionRegex = new Regex(@"^([a-z,A-Z,0-9,\ ]*<{2})?([a-z,A-Z,0-9,\ ]*<)?([a-z,A-Z,0-9,\ ]+)(>[a-z,A-Z,0-9,\ ]*)?(>{2}[a-z,A-Z,0-9,\ ]*)?$");

        internal void GetTypes(
            MethodInfo method,
            out Type[] newLeftTypes,
            out Type[] leftTypes,
            out Type[] strictTypes,
            out Type[] rightTypes,
            out Type[] newRightTypes)
        {
            if (method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException(string.Format(
                    "ProductionAttribute: Production rule must return object (class: {0}, method: {1}).", 
                    method.DeclaringType.Name, 
                    method.ToString()));
            }

            if (0 == method.GetParameters().Count())
            {
                throw new InvalidOperationException(string.Format(
                    "ProductionAttribute: Production rule must have parameters (class: {0}, method: {1}).", 
                    method.DeclaringType.Name, 
                    method.ToString()));
            }

            string[] newLeft, left, strict, right, newRight;

            if (this.Schema == string.Empty)
            {
                newLeft = left = right = newRight = new string[] {};
                strict = (from ParameterInfo p in method.GetParameters() select p.Name).ToArray();
            }
            else
            {
                Match match = productionRegex.Match(this.Schema);
                if (!match.Success)
                {
                    throw new InvalidOperationException(string.Format(
                        "ProductionAttribute: Production string is not correct (Regex match failure, class: {0}, method: {1}).", 
                        method.DeclaringType.Name, 
                        method.ToString()));
                }

                if (match.Groups.Count != 6)
                {
                    throw new InvalidOperationException(string.Format(
                        "ProductionAttribute: Internal error while parsing production (class: {0}, method: {1}).", method.DeclaringType.Name, method.ToString()));
                }

                newLeft = FindNames(match.Groups[1].Value, "<<");
                left = FindNames(match.Groups[2].Value, "<");
                strict = FindNames(match.Groups[3].Value, String.Empty);
                right = FindNames(match.Groups[4].Value, ">");
                newRight = FindNames(match.Groups[5].Value, ">>");            

                var allItems = newLeft.Concat(left).Concat(strict).Concat(right).Concat(newRight).ToArray();
                var methodParametersNames = (from ParameterInfo info in method.GetParameters() select info.Name).ToArray();
                if (!allItems.SequenceEqual(methodParametersNames))
                {
                    throw new InvalidOperationException(string.Format(
                        "ProductionAttribute: List of parameters in production do not match to list of method parameters (class: {0}, method: {1}).", 
                        method.DeclaringType.Name, 
                        method.ToString()));
                }
            }

            var parametersList = method.GetParameters().ToList();

            var ignoreList = IgnoreAttribute.GetIgnoredTypes(method);
            foreach (var p in parametersList)
            {
                if (ignoreList.Contains(p.ParameterType))
                {
                    throw new InvalidOperationException(string.Format(
                        "ProductionAttribute: Ignored type is production function parameters list (class: {0}, method: {1}).", 
                        method.DeclaringType.Name, 
                        method.ToString()));
                }
            }

            newLeftTypes = (from string l in newLeft select parametersList.Find(p => p.Name == l).ParameterType).ToArray();
            leftTypes = (from string l in left select parametersList.Find(p => p.Name == l).ParameterType).ToArray();
            strictTypes = (from string l in strict select parametersList.Find(p => p.Name == l).ParameterType).ToArray();
            rightTypes = (from string l in right select parametersList.Find(p => p.Name == l).ParameterType).ToArray();
            newRightTypes = (from string l in newRight select parametersList.Find(p => p.Name == l).ParameterType).ToArray();            
        }

        private static string[] FindNames(string s, string excludeName)
        {
            return
                (from string name in s.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                 where name != excludeName
                 select name).ToArray();
        }
    }
}

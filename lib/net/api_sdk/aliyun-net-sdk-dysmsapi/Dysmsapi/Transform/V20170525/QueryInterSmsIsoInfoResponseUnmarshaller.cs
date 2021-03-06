/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using Aliyun.Acs.Core.Transform;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System;
using System.Collections.Generic;

namespace Aliyun.Acs.Dysmsapi.Transform.V20170525
{
    public class QueryInterSmsIsoInfoResponseUnmarshaller
    {
        public static QueryInterSmsIsoInfoResponse Unmarshall(UnmarshallerContext context)
        {
			QueryInterSmsIsoInfoResponse queryInterSmsIsoInfoResponse = new QueryInterSmsIsoInfoResponse();

			queryInterSmsIsoInfoResponse.HttpResponse = context.HttpResponse;
			queryInterSmsIsoInfoResponse.RequestId = context.StringValue("QueryInterSmsIsoInfo.RequestId");
			queryInterSmsIsoInfoResponse.Code = context.StringValue("QueryInterSmsIsoInfo.Code");
			queryInterSmsIsoInfoResponse.Message = context.StringValue("QueryInterSmsIsoInfo.Message");

			List<QueryInterSmsIsoInfoResponse.QueryInterSmsIsoInfo_IsoSupportDTO> queryInterSmsIsoInfoResponse_isoSupportDTOs = new List<QueryInterSmsIsoInfoResponse.QueryInterSmsIsoInfo_IsoSupportDTO>();
			for (int i = 0; i < context.Length("QueryInterSmsIsoInfo.IsoSupportDTOs.Length"); i++) {
				QueryInterSmsIsoInfoResponse.QueryInterSmsIsoInfo_IsoSupportDTO isoSupportDTO = new QueryInterSmsIsoInfoResponse.QueryInterSmsIsoInfo_IsoSupportDTO();
				isoSupportDTO.CountryName = context.StringValue("QueryInterSmsIsoInfo.IsoSupportDTOs["+ i +"].CountryName");
				isoSupportDTO.CountryCode = context.StringValue("QueryInterSmsIsoInfo.IsoSupportDTOs["+ i +"].CountryCode");
				isoSupportDTO.IsoCode = context.StringValue("QueryInterSmsIsoInfo.IsoSupportDTOs["+ i +"].IsoCode");

				queryInterSmsIsoInfoResponse_isoSupportDTOs.Add(isoSupportDTO);
			}
			queryInterSmsIsoInfoResponse.IsoSupportDTOs = queryInterSmsIsoInfoResponse_isoSupportDTOs;
        
			return queryInterSmsIsoInfoResponse;
        }
    }
}